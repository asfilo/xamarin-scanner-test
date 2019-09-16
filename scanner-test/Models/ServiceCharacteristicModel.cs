using System;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.Generic;

namespace scannertest.Models
{
    public class ServiceCharacteristicModel
    {
        public IService Service { get; set; }
        public IList<ICharacteristic> Characteristics { get; set; }
    }
}
