using Newtonsoft.Json.Linq;

namespace MKLUODDD.Model.DTO.Forms {

    public class ServiceInfoForm : Form<object> {

        public string? id;

        public ServiceInfoForm() {}
        public ServiceInfoForm(JObject json) : base(json) {
            id = json["id"].ToString();
        }

        public override object Compose() => new object();
    }
}