using Newtonsoft.Json.Linq;

namespace MKLUODDD.Model.DTO.Views {

    public enum OutputType {
        LoginInfo,
        ServiceInfo,
        ActionInfo,
        ServiceOutput,
        Exception
    }

    public abstract class AppOutput {
        public abstract OutputType Type { get; }
        public abstract object Payload { get; }
        public JObject ToJson() => JObject.FromObject(new {
            Type = Type.ToString(),
            Payload
        });
    }
}