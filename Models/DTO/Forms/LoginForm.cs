using System;
using Newtonsoft.Json.Linq;

namespace MKLUODDD.Model.DTO.Forms {

    public class LoginForm : Form<object> {

        public string Username { get; } = "";
        public string Password { get; } = "";
        // public bool Legacy { get; } = false;

        public LoginForm() { }
        public LoginForm(JObject json) : base(json) {
            Username = json["username"]?.ToString() ?? "";
            Password = json["password"]?.ToString() ?? "";
            // Legacy = json["legacy"]?.ToString() == "true";
        }

        public static implicit operator LoginForm(JObject json) => new LoginForm(json);
    }
}