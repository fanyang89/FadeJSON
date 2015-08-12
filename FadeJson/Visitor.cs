using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace FadeJson
{
    public class Visitor
    {
        public dynamic VisitJson(JsonParser.JsonContext context) {
            var obj = context.@object();
            if (obj != null) {
                return VisitObject(obj);
            }

            var array = context.array();
            if (array != null) {
                return VisitArray(array);
            }
            throw new ArgumentException();
        }

        private List<JsonObject> VisitArray(JsonParser.ArrayContext context) {
            JsonParser.ValueContext valueContext;
            var i = 0;
            var list = new List<JsonObject>();

            do {
                valueContext = context.value(i++);
                if (valueContext == null) {
                    break;
                }
                var value = VisitValue(valueContext);
                list.Add(value);
            } while (valueContext != null);

            return list;
        }

        private JsonObject VisitObject(JsonParser.ObjectContext context) {
            var i = 0;
            JsonParser.PairContext pairContext;
            var o = new JsonObject();

            do {
                pairContext = context.pair(i++);
                if (pairContext == null) {
                    break;
                }
                var name = pairContext.STRING().GetText().TrimHeadAndTail();
                var value = VisitValue(pairContext.value());
                o.AddKeyValue(name, value);
            } while (pairContext != null);
            return o;
        }

        private dynamic VisitValue(JsonParser.ValueContext context) {
            var str = context.STRING();
            if (str != null) {
                return str.GetText().TrimHeadAndTail();
            }

            var number = context.NUMBER();
            if (number != null) {
                int result;
                var parseResult = int.TryParse(number.GetText(), out result);
                if (parseResult) {
                    return result;
                }
                throw new ArgumentOutOfRangeException("输入数值不合法！");
            }

            var obj = context.@object();
            if (obj != null) {
                return VisitObject(obj);
            }

            var array = context.array();
            if (array != null) {
                return VisitArray(array);
            }

            var b = context.GetText();
            switch (b) {
                case "true":
                    return true;
                case "false":
                    return false;
            }

            throw new ArgumentException();
        }
    }
}
