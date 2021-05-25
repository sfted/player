using Player.Core.Utils.MVVM;

namespace Player.Core.Entities
{
    public class BaseEntity : Notifier
    {
        public int Id { get; set; }
    }
}
