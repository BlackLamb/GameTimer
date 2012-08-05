using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoGo;

namespace GameTimer
{
    public class Program
    {
        const int scale = 15;
        static bool running = false;
        static bool pause = false;
        static int startTime;
        static int elapsedTime = 0;
        static Thread timerLoop;

        // Gobus modules
        static NetduinoGo.RgbLed led1;
        static NetduinoGo.RgbLed led2;
        static NetduinoGo.RgbLed led3;
        static NetduinoGo.Button button1;
        static NetduinoGo.Button button2;

        public static void Main()
        {
            // Setup GoBus ports
            led1 = new NetduinoGo.RgbLed(GoSockets.Socket8);
            led2 = new NetduinoGo.RgbLed(GoSockets.Socket7);
            led3 = new NetduinoGo.RgbLed(GoSockets.Socket6);
            button1 = new NetduinoGo.Button(GoSockets.Socket1);
            button2 = new NetduinoGo.Button(GoSockets.Socket3);

            // Register Buttons
            button1.ButtonPressed += new NetduinoGo.Button.ButtonEventHandler(button1_ButtonPressed);
            button2.ButtonPressed += new NetduinoGo.Button.ButtonEventHandler(button2_ButtonPressed);

            // Main thread sleep time
            Thread.Sleep(Timeout.Infinite);
        }

        // Start/Stop button
        static void button2_ButtonPressed(object sender, bool buttonState)
        {
            if (!running)
                StartTimmer();
            else if (pause && running)
                ResumeTimer();
            else
                PauseTimer();
        }

        private static void PauseTimer()
        {
            pause = true;
        }

        private static void ResumeTimer()
        {
            pause = false;
        }

        private static void StartTimmer()
        {
            if (timerLoop != null)
                timerLoop.Abort();

            timerLoop = new Thread(TimerThread);
            timerLoop.Start();
        }

        // Reset Button
        static void button1_ButtonPressed(object sender, bool buttonState)
        {
            Reset();
        }

        private static void Reset()
        {
            StopThread();
        }

        private static void StopThread()
        {
            if (timerLoop != null)
                timerLoop.Abort();
        }

        static void TimerThread()
        {
            try
            {
                startTime = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                int lastTime = startTime;
                int currentTime = startTime;
                running = true;

                while (running)
                {
                    currentTime = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                    if (!pause)
                    {
                        elapsedTime += currentTime - lastTime;
                        lastTime = currentTime;

                        if (elapsedTime > 5000 * scale)
                        {
                            led1.SetColor(255, 0, 0);
                            led2.SetColor(255, 0, 0);
                            led3.SetColor(255, 0, 0);
                            running = false;
                        }
                        else if (elapsedTime > 4500 * scale)
                        {
                            led1.SetColor(0, 0, 0);
                            led2.SetColor(0, 0, 0);
                            led3.SetColor(255, 255, 0);
                        }
                        else if (elapsedTime > 4000 * scale)
                        {
                            led1.SetColor(0, 0, 0);
                            led2.SetColor(255, 255, 0);
                            led3.SetColor(255, 255, 0);
                        }
                        else if (elapsedTime > 3000 * scale)
                        {
                            led1.SetColor(255, 255, 0);
                            led2.SetColor(255, 255, 0);
                            led3.SetColor(255, 255, 0);
                        }
                        else if (elapsedTime > 2000 * scale)
                        {
                            led1.SetColor(0, 0, 0);
                            led2.SetColor(0, 0, 0);
                            led3.SetColor(0, 255, 0);
                        }
                        else if (elapsedTime > 1000 * scale)
                        {
                            led1.SetColor(0, 0, 0);
                            led2.SetColor(0, 255, 0);
                            led3.SetColor(0, 255, 0);
                        }
                        else if (elapsedTime <= 1000 * scale)
                        {
                            led1.SetColor(0, 255, 0);
                            led2.SetColor(0, 255, 0);
                            led3.SetColor(0, 255, 0);
                        }
                    }
                    else
                        lastTime = currentTime;
                }

                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadAbortException ex)
            {
                led1.SetColor(0, 0, 0);
                led2.SetColor(0, 0, 0);
                led3.SetColor(0, 0, 0);
                elapsedTime = 0;
                startTime = 0;
            }
            finally { }
        }

    }
}
