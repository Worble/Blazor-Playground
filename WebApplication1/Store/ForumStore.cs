using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;

namespace WebApplication1.Store
{
    public class ForumStore
    {
        private HttpClient _client;
        const string API = "http://localhost:14190/api/";

        public ForumStore(HttpClient client)
        {
            this._client = client;
        }

        public BoardDTO[] Boards { get; private set; }
        public BoardDTO Board { get; private set; }

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public async Task GetAllBoards() { 
            Boards = await _client.GetJsonAsync<BoardDTO[]>(API + "boards/");
            NotifyStateChanged();
        }

        public async Task GetAllThreadsForBoard(int boardId)
        {
            var result = await _client.GetJsonAsync<BoardDTO>(API + "boards/" + boardId + "/threads/");
            if (Board != null && Board.thread != null)
            {
                result.thread = Board.thread;
            }
            Board = result;
            NotifyStateChanged();
        }

        public async Task GetAllPostsForThread(int boardId, int threadId)
        {
            var result = await _client.GetJsonAsync<BoardDTO>(API + "boards/" + boardId + "/threads/" + threadId + "/posts/");
            if (Board != null && Board.threads != null)
            {
                result.threads = Board.threads;
            }
            Board = result;
            NotifyStateChanged();
        }

        public async Task PostThread(int boardId, string content)
        {
            var result = await _client.SendJsonAsync<BoardDTO>(
                HttpMethod.Post,
                API + "boards/" + boardId + "/threads/",
                new ThreadDTO()
                {
                    boardId = boardId,
                    posts = new List<PostDTO>()
                    {
                        new PostDTO() { content = content }
                    }
                });

            if (Board == null)
            {
                Board = result;
            }
            else
            {
                Board.id = result.id;
                Board.thread = result.thread;
            }
            NotifyStateChanged();
        }

        public async Task PostPost(int boardId, int threadId, string content)
        {
            var result = await _client.SendJsonAsync<BoardDTO>(
                HttpMethod.Post,
                API + "boards/" + boardId + "/threads/" + threadId + "/posts/",
                new PostDTO() { content = content, threadId = threadId }
                );

            if (Board == null)
            {
                Board = result;
            }
            else
            {
                Board.id = result.id;
                Board.thread = result.thread;
            }
            NotifyStateChanged();
        }
    }

    public class BoardDTO : BaseDTO
    {
        public string name;
        public string shorthandName;
        public IEnumerable<ThreadDTO> threads;
        public ThreadDTO thread;
    }

    public class ThreadDTO : BaseDTO
    {
        public IEnumerable<PostDTO> posts;
        public int boardId;
    }

    public class PostDTO : BaseDTO
    {
        public string content;
        public int threadId;
        public bool isOp;
    }

    public class BaseDTO
    {
        public int id;
        public DateTime createdDate;
        public DateTime? editedDate;
    }
}
