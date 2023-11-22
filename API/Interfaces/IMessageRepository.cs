using API.DTOs;
using API.Entities;
using API.Helper;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);

    void DeleteMessage(Message message);

    Task<Message> GetMessage(int id);

    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams); // treate this like an unread message inbox and outbox

    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);

    Task<bool> SaveAllAsync();
     
}