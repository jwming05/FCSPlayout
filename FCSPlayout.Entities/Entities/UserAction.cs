using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCSPlayout.Entities
{
    [Table("UserActions")]
    [Serializable]
    public class UserAction : ICreationTimestamp, IGuidIdentifier//, IObjectWithState
    {
        [Key]
        public Guid Id { get; set; }

        // ID, 用户，描述，类型, 时间, Tag, Data

        public byte[] Data { get; set; }

        public string Tag { get; set; }

        public string Description { get; set; }

        public UserActionCategory Category { get; set; }

        public DateTime CreationTime
        {
            get; set;
        }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }

        public string ApplicationName { get; set; }

        public string Name { get; set; }
    }

    public enum UserActionCategory
    {
        Unknown = 0,
        Login,
        Logout,
        Add,
        Remove,
        Modify
    }
}
