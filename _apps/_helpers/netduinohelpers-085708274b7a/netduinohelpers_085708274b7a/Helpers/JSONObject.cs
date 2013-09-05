using System;

namespace netduino.helpers.Helpers {
    public enum JSONObjectType {
        String,
        Array,
        Object
    }

    public class JSONObject {
        public JSONObjectType ObjectType { get; set; }
        public Object Object { get; set; }

        public JSONObject(Object obj, JSONObjectType type) {
            ObjectType = type;
            Object = obj;
        }
    }
}
