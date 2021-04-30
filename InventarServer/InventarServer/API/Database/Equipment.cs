using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace InventarAPI
{
    class Equipment
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string SecondNumber { get; set; }
        public string CurrentNumber { get; set; }
        public string ActivationDate { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public double Cost { get; set; }
        public double BookValue { get; set; }
        public char Currency { get; set; }
        public string KfzLicensePlate { get; set; }
        public string Room { get; set; }
        public string RoomName { get; set; }

        /// <summary>
        /// Is used to be able to store a DatabaseLocations in a json-File
        /// </summary>
        public Equipment() { }

        /// <summary>
        /// Saves Values
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_number"></param>
        /// <param name="_secondNumber"></param>
        /// <param name="_currentNumber"></param>
        /// <param name="_activationDate"></param>
        /// <param name="_name"></param>
        /// <param name="_serialNumber"></param>
        /// <param name="_cost"></param>
        /// <param name="_bookValue"></param>
        /// <param name="_currency"></param>
        /// <param name="_kfzLicensePlate"></param>
        /// <param name="_room"></param>
        /// <param name="_roomName"></param>
        public Equipment(int _id, string _number, string _secondNumber, string _currentNumber, string _activationDate,
            string _name, string _serialNumber, double _cost, double _bookValue, char _currency, string _kfzLicensePlate,
            string _room, string _roomName)
        {
            ID = _id;
            Number = _number;
            SecondNumber = _secondNumber;
            CurrentNumber = _currentNumber;
            ActivationDate = _activationDate;
            Name = _name;
            SerialNumber = _serialNumber;
            Cost = _cost;
            BookValue = _bookValue;
            Currency = _currency;
            KfzLicensePlate = _kfzLicensePlate;
            Room = _room;
            RoomName = _roomName;
        }
          
        public byte[] ToByteArray()
        {
            ASCIIEncoding an = new ASCIIEncoding();
            string json = JsonSerializer.Serialize(this);
            return an.GetBytes(json);
        }

        public Equipment FromByteArray(byte[] _data)
        {
            ASCIIEncoding an = new ASCIIEncoding();
            string json = an.GetString(_data);
            return JsonSerializer.Deserialize<Equipment>(json);
        }

        /// <summary>
        /// Returns the Equipment as a beautiful string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(" - ID             : " + ID);
            b.AppendLine(" - Number         : " + Number);
            b.AppendLine(" - SecondNumber   : " + SecondNumber);
            b.AppendLine(" - CurrentNumber  : " + CurrentNumber);
            b.AppendLine(" - ActivationDate : " + ActivationDate);
            b.AppendLine(" - Name           : " + Name);
            b.AppendLine(" - SerialNumber   : " + SerialNumber);
            b.AppendLine(" - Cost           : " + Cost);
            b.AppendLine(" - BookValue      : " + BookValue);
            b.AppendLine(" - Currency       : " + Currency);
            b.AppendLine(" - KfzLicensePlate: " + KfzLicensePlate);
            b.AppendLine(" - Room           : " + Room);
            b.AppendLine(" - RoomName       : " + RoomName);
            return b.ToString();
        }
    }
}
