using CPositiveAPI.Model;

namespace CPositiveAPI.Interfaces
{
    public interface IForumRepository
    {
        // Topics
        IEnumerable<Topic> GetTopics();
        Topic GetTopicById(int id);
        void AddTopic(Topic topic);

        // Discussions
        IEnumerable<Discussion> GetDiscussionsByTopicId(int topicId);
        void AddDiscussion(Discussion discussion);

        // Profile Image
        Task<string?> GetProfileImageByUserId(int userId);

        // Comments count
        Task<(int commentCount, List<int> userIds)> GetCommentCountAndUserIds(int topicId);

        // Search Topics
        Task<IEnumerable<object>> SearchTopics(string query);

        // Save changes
        bool Save();
    }
}
