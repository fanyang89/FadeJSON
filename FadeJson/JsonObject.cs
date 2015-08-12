using System;
using System.Collections.Generic;

namespace FadeJson
{
    public class JsonObject
    {
        private readonly Dictionary<string, dynamic> _dict = new Dictionary<string, dynamic>();

        public void AddKeyValue(string key, dynamic value) {
            _dict.Add(key, value);
        }

        public dynamic this[string key] {
            get {
                return _dict[key];
            }
            set {
                _dict[key] = value;
            }
        }

    }
}