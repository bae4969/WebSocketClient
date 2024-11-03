using Microsoft.Maui.Layouts;
using System.Numerics;

namespace WebSocketClient.Pages;

public partial class LevelerPage : ContentPage
{
	private bool _isListenGeometry = false;
	private Queue<Vector3> _accVecList = [];


	public LevelerPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (Accelerometer.Default.IsSupported)
		{
			if (_isListenGeometry) return;

			Accelerometer.Default.ReadingChanged += AccelerometerReadingChanged;
			Accelerometer.Default.Start(SensorSpeed.UI);
			DeviceDisplay.KeepScreenOn = true;
			_isListenGeometry = true;
		}
		else
		{
			await Application.Current.MainPage.DisplayAlert("Error", "Not supported device.", "OK");
			Navigation.PopAsync();
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		// Accelerometer 중지 및 이벤트 핸들러 제거
		if (!_isListenGeometry) return;
		Accelerometer.Default.Stop();
		Accelerometer.Default.ReadingChanged -= AccelerometerReadingChanged;
		DeviceDisplay.KeepScreenOn = false;
		_isListenGeometry = false;
	}

	// Accelerometer에서 데이터를 읽어올 때마다 호출되는 메서드
	private void AccelerometerReadingChanged(object sender, AccelerometerChangedEventArgs e)
	{
		_accVecList.Enqueue(e.Reading.Acceleration);
		if (_accVecList.Count > 10)
			_accVecList.Dequeue();

		Thickness margin;
		Vector3 vec = new(0,0,0);
		foreach (var tt in _accVecList)
			vec += tt;
		vec /= _accVecList.Count;

		double tilt_deg = Math.Atan2(vec.Y, Math.Abs(vec.Z)) * (180.0 / Math.PI);
		double roll_deg = Math.Atan2(vec.X, Math.Abs(vec.Z)) * (180.0 / Math.PI);

		double tilt_rate = -tilt_deg / 90.0;
		double roll_rate = roll_deg / 90.0;
		double tot_rate = Math.Sqrt(Math.Pow(tilt_rate, 2) + Math.Pow(roll_rate, 2));

		bool is_tilt_over = Math.Abs(tilt_deg) > 2.0;
		bool is_roll_over = Math.Abs(roll_deg) > 2.0;

		margin = TiltDot.Margin;
		margin.Top = 110.0 + 100.0 * tilt_rate;
		TiltDot.Margin = margin;

		margin = RollDot.Margin;
		margin.Left = 110.0 + 100.0 * roll_rate;
		RollDot.Margin = margin;

		margin = TotDot.Margin;
		if (tot_rate > 1.0)
		{
			margin.Top = 110.0 + 100.0 * tilt_rate / tot_rate;
			margin.Left = 110.0 + 100.0 * roll_rate / tot_rate;
		}
		else
		{
			margin.Top = 110.0 + 100.0 * tilt_rate;
			margin.Left = 110.0 + 100.0 * roll_rate;
		}
		TotDot.Margin = margin;

		TiltDot.BackgroundColor = is_tilt_over ? Color.FromRgba(0, 0, 255, 255) : Color.FromRgba(255, 0, 0, 255);
		RollDot.BackgroundColor = is_roll_over ? Color.FromRgba(0, 0, 255, 255) : Color.FromRgba(255, 0, 0, 255);
		TotDot.BackgroundColor = is_tilt_over | is_roll_over ? Color.FromRgba(0, 0, 255, 255) : Color.FromRgba(255, 0, 0, 255);

		DegreeTextObj.Text = $"X : {Math.Abs(tilt_deg):F2}\n" + $"Y : {Math.Abs(roll_deg):F2}";
	}
}