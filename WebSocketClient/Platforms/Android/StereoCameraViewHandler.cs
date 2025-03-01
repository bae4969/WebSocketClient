using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orientation = Android.Widget.Orientation;

namespace WebSocketClient.Views
{// MAUI용 ViewHandler: (가상) StereoCameraView -> (실제) Android ViewGroup(프레임레이아웃) 
	public class StereoCameraViewHandler : ViewHandler<StereoCameraView, LinearLayout>
	{
		public static IPropertyMapper<StereoCameraView, StereoCameraViewHandler> StereoCameraViewMapper =
			new PropertyMapper<StereoCameraView, StereoCameraViewHandler>(ViewHandler.ViewMapper);

		public static CommandMapper<StereoCameraView, StereoCameraViewHandler> StereoCameraViewCommandMapper =
			new(ViewHandler.ViewCommandMapper);

		public StereoCameraViewHandler()
			: base(StereoCameraViewMapper, StereoCameraViewCommandMapper) // ✅ 기존 null 제거
		{
		}


		CameraManager? _cameraManager;
		string[]? _targetCameraIds;
		CameraDevice? _cameraDevice1;
		CameraDevice? _cameraDevice2;

		private TextureView? _textureView1;
		private TextureView? _textureView2;
		private ImageReader _imageReader1;
		private ImageReader _imageReader2;

		CameraCaptureSession? _captureSession1;
		CameraCaptureSession? _captureSession2;

		HandlerThread? _cameraThread1;
		HandlerThread? _cameraThread2;
		Android.OS.Handler? _cameraHandler1;
		Android.OS.Handler? _cameraHandler2;

		protected override LinearLayout CreatePlatformView() {
			var container = new LinearLayout(Context);
			container.Orientation = Orientation.Vertical; // 세로로 배치

			// 전면 카메라 (상단)
			_textureView1 = new TextureView(Context);
			_textureView1.LayoutParameters = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.MatchParent, 0, 1); // 50% 높이

			// 후면 카메라 (하단)
			_textureView2 = new TextureView(Context);
			_textureView2.LayoutParameters = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.MatchParent, 0, 1); // 50% 높이

			container.AddView(_textureView1);
			container.AddView(_textureView2);

			return container;
		}

		protected override void ConnectHandler(LinearLayout platformView) {
			base.ConnectHandler(platformView);

			// 안드로이드 카메라 매니저 얻기
			_cameraManager = Context?.GetSystemService(Context.CameraService) as CameraManager;

			// 가능한 전면 카메라 ID를 가져오기
			if (_cameraManager != null) {
				if (Build.VERSION.SdkInt >= BuildVersionCodes.R &&
					_cameraManager.ConcurrentCameraIds.Count > 0) {
					_targetCameraIds = _cameraManager.ConcurrentCameraIds.ToList()[0].ToArray();
				}
				else {
					_targetCameraIds = _cameraManager.GetCameraIdList();
				}
			}

			// TextureView가 화면에 표시될 준비가 되면 카메라를 열도록 리스너 등록
			_textureView1!.SurfaceTextureListener = new MyTextureListener(this, cameraIndex: 0);
			_textureView2!.SurfaceTextureListener = new MyTextureListener(this, cameraIndex: 1);
		}

		protected override void DisconnectHandler(LinearLayout platformView) {
			// 리소스 해제
			CloseAllCameras();
			base.DisconnectHandler(platformView);
		}

		// TextureView가 사용 가능한 상태일 때 카메라 열기
		internal void OpenCamera(int cameraIndex, SurfaceTexture surfaceTexture) {
			if (_cameraManager == null || _targetCameraIds == null) return;
			if (cameraIndex >= _targetCameraIds.Length) return;
			var cameraId = _targetCameraIds[cameraIndex];

			// 카메라용 백그라운드 스레드 시작
			if (cameraIndex == 0 && _cameraThread1 == null) {
				_cameraThread1 = new HandlerThread("CameraThread1");
				_cameraThread1.Start();
				_cameraHandler1 = new Android.OS.Handler(_cameraThread1.Looper);
				_imageReader1 = ImageReader.NewInstance(1920, 1080, ImageFormatType.Jpeg, 2);
				_imageReader1.SetOnImageAvailableListener(new MyImageAvailableListener("front_camera"), _cameraHandler1);
			}
			if (cameraIndex == 1 && _cameraThread2 == null) {
				_cameraThread2 = new HandlerThread("CameraThread2");
				_cameraThread2.Start();
				_cameraHandler2 = new Android.OS.Handler(_cameraThread2.Looper);
				_imageReader2 = ImageReader.NewInstance(1920, 1080, ImageFormatType.Jpeg, 2);
				_imageReader2.SetOnImageAvailableListener(new MyImageAvailableListener("front_camera"), _cameraHandler2);
			}

			var handler = (cameraIndex == 0) ? _cameraHandler1 : _cameraHandler2;
			try {
				_cameraManager.OpenCamera(cameraId, new MyCameraStateCallback(this, cameraIndex), handler);
			}
			catch (System.Exception ex) {
				System.Diagnostics.Debug.WriteLine($"OpenCamera Error: {ex}");
			}
		}

		private void CloseAllCameras() {
			try {
				_captureSession1?.StopRepeating();
				_captureSession1?.Close();
				_captureSession1 = null;
			}
			catch { }

			try {
				_captureSession2?.StopRepeating();
				_captureSession2?.Close();
				_captureSession2 = null;
			}
			catch { }

			_cameraDevice1?.Close();
			_cameraDevice1 = null;
			_cameraDevice2?.Close();
			_cameraDevice2 = null;

			_cameraThread1?.QuitSafely();
			_cameraThread1 = null;
			_cameraThread2?.QuitSafely();
			_cameraThread2 = null;
		}

		// 카메라가 열렸을 때 CameraDevice 객체를 저장하고 바로 세션 생성
		internal void OnCameraOpened(CameraDevice cameraDevice, int cameraIndex) {
			if (cameraIndex == 0)
				_cameraDevice1 = cameraDevice;
			else
				_cameraDevice2 = cameraDevice;

			// 카메라 세션 구성
			CreateCameraPreviewSession(cameraIndex);
		}

		private void CreateCameraPreviewSession(int cameraIndex) {
			var cameraDevice = (cameraIndex == 0) ? _cameraDevice1 : _cameraDevice2;
			if (cameraDevice == null) return;

			var textureView = (cameraIndex == 0) ? _textureView1 : _textureView2;
			var handler = (cameraIndex == 0) ? _cameraHandler1 : _cameraHandler2;
			if (textureView == null || textureView.IsAvailable == false) return;

			var surfaceTexture = textureView.SurfaceTexture;
			// 미리보기 해상도 지정 가능 (기본으로 TextureView 크기 사용)
			surfaceTexture.SetDefaultBufferSize(textureView.Width, textureView.Height);
			var previewSurface = new Surface(surfaceTexture);

			var surfaces = new JavaList<Surface> { previewSurface };

			// 세션 생성 콜백
			var sessionCallback = new MySessionStateCallback(this, cameraIndex);

			cameraDevice.CreateCaptureSession(surfaces, sessionCallback, handler);
		}

		// 세션이 구성되면 프리뷰 요청을 보냄
		internal void OnSessionConfigured(CameraCaptureSession session, int cameraIndex) {
			if (cameraIndex == 0)
				_captureSession1 = session;
			else
				_captureSession2 = session;

			var cameraDevice = (cameraIndex == 0) ? _cameraDevice1 : _cameraDevice2;
			if (cameraDevice == null) return;

			try {
				var requestBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
				var textureView = (cameraIndex == 0) ? _textureView1 : _textureView2;
				var surface = new Surface(textureView!.SurfaceTexture);

				requestBuilder.AddTarget(surface);
				requestBuilder.Set(CaptureRequest.ControlMode, (int)ControlMode.Auto);

				// 지속적 미리보기
				session.SetRepeatingRequest(requestBuilder.Build(), null,
					(cameraIndex == 0) ? _cameraHandler1 : _cameraHandler2);
			}
			catch (CameraAccessException ex) {
				System.Diagnostics.Debug.WriteLine($"OnSessionConfigured Error: {ex}");
			}
		}

		// --- 아래 내부 콜백 클래스들 ---

		// TextureView 콜백
		private class MyTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
		{
			private readonly StereoCameraViewHandler _handler;
			private readonly int _cameraIndex;

			public MyTextureListener(StereoCameraViewHandler handler, int cameraIndex) {
				_handler = handler;
				_cameraIndex = cameraIndex;
			}

			public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height) {
				_handler.OpenCamera(_cameraIndex, surface);
			}

			public bool OnSurfaceTextureDestroyed(SurfaceTexture surface) {
				return true;
			}

			public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height) { }
			public void OnSurfaceTextureUpdated(SurfaceTexture surface) { }
		}

		// CameraDevice 상태 콜백
		private class MyCameraStateCallback : CameraDevice.StateCallback
		{
			private readonly StereoCameraViewHandler _handler;
			private readonly int _cameraIndex;

			public MyCameraStateCallback(StereoCameraViewHandler handler, int cameraIndex) {
				_handler = handler;
				_cameraIndex = cameraIndex;
			}

			public override void OnOpened(CameraDevice camera) {
				_handler.OnCameraOpened(camera, _cameraIndex);
			}

			public override void OnDisconnected(CameraDevice camera) {
				camera.Close();
			}

			public override void OnError(CameraDevice camera, CameraError error) {
				camera.Close();
			}
		}

		// 세션 구성 콜백
		private class MySessionStateCallback : CameraCaptureSession.StateCallback
		{
			private readonly StereoCameraViewHandler _handler;
			private readonly int _cameraIndex;

			public MySessionStateCallback(StereoCameraViewHandler handler, int cameraIndex) {
				_handler = handler;
				_cameraIndex = cameraIndex;
			}

			public override void OnConfigured(CameraCaptureSession session) {
				_handler.OnSessionConfigured(session, _cameraIndex);
			}

			public override void OnConfigureFailed(CameraCaptureSession session) {
				session.Close();
			}
		}

		public class MyImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
		{
			private string _fileName;

			public MyImageAvailableListener(string fileName) {
				_fileName = fileName;
			}

			public void OnImageAvailable(ImageReader reader) {
				var image = reader.AcquireLatestImage();
				if (image == null) return;

				//// 이미지 저장
				//var buffer = image.GetPlanes()[0].Buffer;
				//byte[] bytes = new byte[buffer.Remaining()];
				//buffer.Get(bytes);

				image.Close();
			}
		}
	}
}
