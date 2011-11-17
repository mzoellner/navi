using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Navi.VibrationProcessing
{
    public enum VibratorPosition
    {
        Left,
        Center,
        Right
    }

    public class VibrationManager
    {
        private SerialPort _serialPort;

        private Dictionary<VibratorPosition, String> _prefixes = new Dictionary<VibratorPosition, string>();

        private Dictionary<VibratorPosition, int> _delays = new Dictionary<VibratorPosition, int>();

        private SerialPort GetSerialPort(String portName)
        {
            return new SerialPort(portName);
        }

        public VibrationManager(String portName)
        {
            _prefixes[VibratorPosition.Left] = "l,";
            _prefixes[VibratorPosition.Center] = "c,";
            _prefixes[VibratorPosition.Right] = "r,";

            _delays[VibratorPosition.Left] = 5000;
            _delays[VibratorPosition.Center] = 5000;
            _delays[VibratorPosition.Right] = 5000;

            try
            {
                _serialPort = GetSerialPort(portName);
                _serialPort.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public int GetDelay(VibratorPosition position)
        {
            return _delays[position];
        }

        public void ChangeDelay(VibratorPosition position, int delay)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _delays[position] = delay;
                _serialPort.WriteLine(_prefixes[position] + delay);
            }
        }

    }
}
