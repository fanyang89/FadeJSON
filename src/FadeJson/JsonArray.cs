using System;
using System.Collections;
using System.Collections.Generic;

namespace FadeJSON
{
    public class JsonArray : IEnumerable<JsonObject>
    {
        private readonly List<JsonObject> _list;

        public JsonArray() {
            _list = new List<JsonObject>();
        }

        public JsonArray(List<JsonObject> list) {
            _list = list;
        }

        public void Add(JsonObject j) {
            _list.Add(j);
        }

        public static implicit operator JsonObject(JsonArray array) {
            return new JsonObject(array._list);
        }

        public static implicit operator JsonArray(JsonObject arrayObject) {
            if (arrayObject.Type != JsonObjectType.Array) {
                throw new InvalidCastException();
            }
            return new JsonArray(arrayObject.Array);
        }

        public IEnumerator<JsonObject> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}