namespace API.Interfaces;

public interface IUnitOfWork
{
    // Unit of work is like a transaction. If we had multi different updates that we needed to take place in our DB
    // If one of them fails, then they should all fail. 
    // So at the end of those of that transaction. then we'd complete -> if the complete doesn't work, then everything rolls back to what it was before

    /* Hint: Ở EF khi ta lấy dữ liệu của entity từ DB và gán nó vào 1 con trỏ tham chiếu tới vùng nhớ đc query đó.
        Sau đó ta thực hiện thay đổi dữ liệu của entity đó tiếp theo ta thực hiện SaveChanges() lưu tất cả 
        các thay đổi vào DB thì con trỏ ban đầu trỏ tới vùng dữ liệu query hồi trước thì dữ liệu đã được cập nhập đúng với ta mong đợi -> Consistency
    */
    IUserRepository UserRepository { get; }

    IMessageRepository MessageRepository { get; }

    ILikesRepository LikesRepository { get; }

    Task<bool> Complete();

    bool HasChanges(); // tell us if EF is tracking anything that's been changed inside its transaction
}