﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.DataContexts
{
    public class BoardsRepo
    {
        private readonly ApplicationDbContext db;

        public BoardsRepo() : this(new ApplicationDbContext())
        {
        }

        public BoardsRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IEnumerable<BoardModel> GetBoards()
        {
            return db.Boards;
        }

        public async Task<BoardModel> GetBoard(string boardId)
        {
            return await db.Boards.FindAsync(boardId);
        }

        public async Task AddBoard(BoardModel board)
        {
            db.Boards.Add(board);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception(
                    string.Join("\r\n",
                        e.EntityValidationErrors.SelectMany(v => v.ValidationErrors)
                            .Select(err => err.PropertyName + " " + err.ErrorMessage)));
            }
        }

        public async Task DeleteBoard(string boardId)
        {
            var threads = db.Threads.Where(t => t.BoardId == boardId).ToList();
            var posts = db.Posts.Where(p => threads.Any(t => t.Id == p.ThreadId));
            db.Posts.RemoveRange(posts);
            db.Threads.RemoveRange(threads);
            var board = db.Boards.Find(boardId);
            if (board != null)
                db.Boards.Remove(board);
            await db.SaveChangesAsync();
        }
    }
}
