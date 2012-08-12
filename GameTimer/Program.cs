//#define mute

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoGo;

namespace GameTimer
{
    public class Program
    {
        static Timer gameTimer;
        static int timeCount = 0;
        static bool blinkOff = false;
        static int timerMin = 5;
        static int scale;
        static timeModes mode = timeModes.five;
        static bool changeMode = true;


        // Gobus modules
        static NetduinoGo.RgbLed led1;
        static NetduinoGo.RgbLed led2;
        static NetduinoGo.RgbLed led3;
        static NetduinoGo.Button button1;
        static NetduinoGo.Button button2;
        static NetduinoGo.PiezoBuzzer buzzer;

        public static void Main()
        {
            // Setup GoBus ports
            led1 = new NetduinoGo.RgbLed(GoSockets.Socket8);
            led2 = new NetduinoGo.RgbLed(GoSockets.Socket7);
            led3 = new NetduinoGo.RgbLed(GoSockets.Socket6);
            button1 = new NetduinoGo.Button(GoSockets.Socket1);
            button2 = new NetduinoGo.Button(GoSockets.Socket3);
            InterruptPort settingButton = new InterruptPort(Pins.Button, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
            settingButton.OnInterrupt += new NativeEventHandler(settingButton_OnInterrupt);
            OutputPort powerlight = new OutputPort(Pins.PowerLed, false);

#if !mute
            buzzer = new NetduinoGo.PiezoBuzzer(); 
#endif

            // Set Scale
            SetScale();

            // Register Buttons
            button1.ButtonPressed += new NetduinoGo.Button.ButtonEventHandler(button1_ButtonPressed);
            button2.ButtonPressed += new NetduinoGo.Button.ButtonEventHandler(button2_ButtonPressed);
            led2.SetColor(0, 0, 255);
            // Main thread sleep time
            Thread.Sleep(Timeout.Infinite);
        }

        private static void SetScale()
        {
            scale = (timerMin * 60) / 5;
        }

        static void settingButton_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            ResetTimer();
            switch (mode)
            {
                case timeModes.two:
                    led1.SetColor(0, 0, 255);
                    break;
                case timeModes.five:
                    led2.SetColor(0, 0, 255);
                    break;
                case timeModes.eight:
                    led3.SetColor(0, 0, 255);
                    break;
                case timeModes.ten:
                    led1.SetColor(0, 0, 255);
                    led2.SetColor(0, 0, 255);
                    led3.SetColor(0, 0, 255);
                    break;
            }
            changeMode = true;
        }

        // Reset Button
        static void button1_ButtonPressed(object sender, bool buttonState)
        {
            if (changeMode)
            {
                mode++;
                if ((int)mode > 3)
                    mode = 0;
                switch (mode)
                {
                    case timeModes.two:
                        timerMin = 2;
                        led1.SetColor(0, 0, 255);
                        led2.SetColor(0, 0, 0);
                        led3.SetColor(0, 0, 0);
                        break;
                    case timeModes.five:
                        timerMin = 5;
                        led1.SetColor(0, 0, 0);
                        led2.SetColor(0, 0, 255);
                        led3.SetColor(0, 0, 0);
                        break;
                    case timeModes.eight:
                        timerMin = 8;
                        led1.SetColor(0, 0, 0);
                        led2.SetColor(0, 0, 0);
                        led3.SetColor(0, 0, 255);
                        break;
                    case timeModes.ten:
                        timerMin = 10;
                        led1.SetColor(0, 0, 255);
                        led2.SetColor(0, 0, 255);
                        led3.SetColor(0, 0, 255);
                        break;
                }
                SetScale();
            }
            else
            {
                ResetTimer();
            }
        }

        // Start/Stop button
        static void button2_ButtonPressed(object sender, bool buttonState)
        {
            changeMode = false;
            if (gameTimer != null)
            {
                ResetTimer();
            }
#if !mute
            buzzer.SetFrequency(1046.5f);
#endif
            gameTimer = new Timer(new TimerCallback(TimerTick), null, 1000 * scale, 500 * scale);
            led1.SetColor(0, 255, 0);
            led2.SetColor(0, 255, 0);
            led3.SetColor(0, 255, 0);
#if !mute
            Thread.Sleep(20);
            buzzer.SetFrequency(0.0f);
#endif
        }

        private static void TimerTick(object o)
        {
            timeCount++;
#if !debug

            Debug.Print("Tick: " + timeCount);
#endif
            if (timeCount == 1) // 1 min has passed
            {
                led1.SetColor(0, 0, 0);
            }
            else if (timeCount == 3) // 2 min has passed
            {
                led2.SetColor(0, 0, 0);
            }
            else if (timeCount == 5) // 3 min has passed
            {
                led1.SetColor(255, 255, 0);
                led2.SetColor(255, 255, 0);
                led3.SetColor(255, 255, 0);
            }
            else if (timeCount == 7) // 4 min has passed
            {
                led1.SetColor(0, 0, 0);
#if !mute
                buzzer.SetFrequency(523.25f);
                Thread.Sleep(20);
                buzzer.SetFrequency(0.0f);
#endif
            }
            else if (timeCount == 8) // 4.5 min pas passed
            {
                led2.SetColor(0, 0, 0);
#if !mute
                buzzer.SetFrequency(523.25f);
                Thread.Sleep(20);
                buzzer.SetFrequency(0.0f);
#endif
            }
            else if (timeCount == 9) // 5 min has passed
            {
                led1.SetColor(255, 0, 0);
                led3.SetColor(0, 0, 0);
#if !mute
                buzzer.SetFrequency(440.0f);
                Thread.Sleep(750);
                buzzer.SetFrequency(0.0f); 
#endif
            }
            else if (timeCount == 10)
            {
                led2.SetColor(255, 0, 0);
            }
            else if (timeCount == 11)
            {
                led3.SetColor(255, 0, 0);
#if !mute
                buzzer.SetFrequency(440.0f);
                Thread.Sleep(750);
                buzzer.SetFrequency(0.0f); 
#endif
            }
            else if (timeCount > 11)
            {
                gameTimer.Change(500, 500);
                if (blinkOff)
                {
                    led1.SetColor(0, 0, 0);
                    led2.SetColor(0, 0, 0);
                    led3.SetColor(0, 0, 0);
                    blinkOff = !blinkOff;
                }
                else
                {
                    led1.SetColor(255, 0, 0);
                    led2.SetColor(255, 0, 0);
                    led3.SetColor(255, 0, 0);
                    blinkOff = !blinkOff;
#if (!mute)
                    buzzer.SetFrequency(440.0f);
                    Thread.Sleep(100);
                    buzzer.SetFrequency(0.0f); 
#endif
                }
            }
        }

        private static void ResetTimer()
        {
            if (gameTimer != null)
                gameTimer.Dispose();
            gameTimer = null;
            led1.SetColor(0, 0, 0);
            led2.SetColor(0, 0, 0);
            led3.SetColor(0, 0, 0);
            timeCount = 0;
        }
    }

    enum timeModes
    {
        two,
        five,
        eight,
        ten
    }
}
