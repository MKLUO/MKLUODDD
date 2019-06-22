using Newtonsoft.Json.Linq;

namespace MKLUODDD.Model.DTO.Forms {
    public abstract class Form<TEntity> where TEntity : new() {

        // Deserialization
        public Form() {}
        public Form(JObject json) {}

        public virtual JObject Serialize() =>
            JObject.FromObject(this);

        public virtual TEntity Compose() =>
            new TEntity();
    }
}

namespace MKLUODDD.Model.DTO.Views {
    // public abstract class View<TEntity> where TEntity : Domain.Entity {

    //     // Decomposition
    //     public View() {}
    //     public View(TEntity entity) {}

    //     // public JObject Serialize() =>
    //     //     JObject.FromObject(this);
    // }
}