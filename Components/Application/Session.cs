using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Http;

namespace MKLUODDD.Apps {

    using Core;
    using Model.Domain;
    using Model.Persist;

    // public interface ICrmSession {
    //     string? TimeCreated { get; set; }
    //     PersistInfo<User>? User { get; set; }
    //     void Clear();
    // }

    public class AppSession {
        static readonly string timeKey = "session.time";
        static readonly string userKey = "session.user";
        static readonly string userVersionKey = "session.userVer";

        readonly IHttpContextAccessor httpContextAccessor;

        public AppSession(IHttpContextAccessor httpContextAccessor) =>
            this.httpContextAccessor = httpContextAccessor;

        ISession Session =>
            httpContextAccessor.HttpContext.Session;

        public string? TimeCreated {
            get => Session.GetString(timeKey);
            set => Session.SetString(timeKey, value);
        }

        public UserPersistInfo? User {
            get {
                var id = Session.GetInt32(userKey) ;
                var version = (Session.GetInt32(userVersionKey) is int ver) ? BitConverter.GetBytes(ver) : null;

                if ((id == null) || (version == null)) 
                    return null;

                return new UserPersistInfo(id.Value, new ByteArray(version));
            }
            set {
                if (value?.Id is int id)
                    Session.SetInt32(userKey, id);
                if (value?.Ver is ByteArray ver)
                    Session.SetInt32(userVersionKey, BitConverter.ToInt32(ver.Data));
            }
        } 

        public int? UserId {
            get => Session.GetInt32(userKey);
            set {  
                if (value.HasValue)
                    Session.SetInt32(userKey, value.Value);
            }
        }

        public byte[]? UserVersion {
            get {
                var ver = Session.GetInt32(userVersionKey);
                if (ver == null)
                    return null;
                else return BitConverter.GetBytes(ver.Value);
            } 
            set {  
                if (value == null) return;

                Session.SetInt32(userVersionKey, BitConverter.ToInt32(value));
            }
        }

        // public Domain.User User {
        //     get => Deserialize<Domain.User>(Session.Get(userKey));
        //     set => Session.Set(userKey, Serialize(value));
        // }

        public void Clear() {
            Session.Clear();
        }

        // byte[] Serialize(object obj) {
        //     BinaryFormatter bf = new BinaryFormatter();
        //     using MemoryStream ms = new MemoryStream();
        //     bf.Serialize(ms, obj);
        //     return ms.ToArray();            
        // }

        // T Deserialize<T>(byte[] byteArray) {
        //     BinaryFormatter bf = new BinaryFormatter();
        //     using MemoryStream ms = new MemoryStream(byteArray);
        //     return (T)bf.Deserialize(ms);           
        // }
    }
}