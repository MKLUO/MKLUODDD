

namespace MKLUODDD.Model.Domain {

    public struct UserMiscData {
        public byte UserStatus { get; }
        public byte UserLevel { get; }
        public string UserNo { get; }
        public int? CompanyId { get; }
        public string? UserEmail { get; }

        public UserMiscData(
            byte userStatus,
            byte userLevel,
            string userNo,
            int? companyId = null,
            string? userEmail = null
        ) {
            UserStatus = userStatus;
            UserLevel = userLevel;
            UserNo = userNo;
            CompanyId = companyId;
            UserEmail = userEmail;
        }

        public static UserMiscData None =>
            new UserMiscData(
                userStatus: 0,
                userLevel: 0,
                userNo: ""
            );
    }

    public interface IUser {
        Name Name { get; }
        Username Username { get; }

        UserMiscData MiscData { get; }

        // byte UserStatus { get; }
        // byte UserLevel { get; }
        // string UserNo { get; }
        // int? CompanyId { get; }
        // string? UserEmail { get; }

        bool HasPassword(Password password);
        // void ChangePassword(Password newPassword);
    }

    public abstract class BaseUser {
        public UserMiscData MiscData { get; protected set; }
        public byte UserStatus => MiscData.UserStatus;
        public byte UserLevel => MiscData.UserLevel;
        public string UserNo => MiscData.UserNo;
        public int? CompanyId => MiscData.CompanyId;
        public string? UserEmail => MiscData.UserEmail;
    }
}