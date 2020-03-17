using System;
using System.Collections.Generic;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Linq;
using CMS.Common;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class BoardService : IBoardService
    {
        readonly IRepository _repository;

        public BoardService(IRepository repository)
        {
            _repository = repository;
        }

        public BoardProjection GetBoardById(int boardId)
        {
            return _repository.Project<Board, BoardProjection>(
                boards => (from b in boards
                           where b.BoardId == boardId
                           select new BoardProjection
                           {
                               BoardId = b.BoardId,
                               Name = b.Name
                           }).FirstOrDefault());
        }

        public CMSResult Delete(int boardId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Board>(b => b.BoardId == boardId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Board '{0}' already exists!", model.Name) });
            }
            else
            {
                var isExists = _repository.Project<Student, bool>(students => (
                                from s in students
                                where s.BoardId == boardId
                                select s
                            ).Any());

                if (isExists)
                {
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Board '{0}'. Because it belongs to student!", model.Name) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Board '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public IEnumerable<BoardProjection> GetBoards()
        {
            return _repository.Project<Board, BoardProjection[]>(
                 boards => (from b in boards
                            select new BoardProjection
                            {
                                BoardId = b.BoardId,
                                Name = b.Name
                            }).ToArray());

        }
        public IEnumerable<BoardProjection> GetAllBoardByPrefix(string prefix)
        {
            return _repository.Project<Board, BoardProjection[]>(
                boards => (from b in boards
                           where b.Name.StartsWith(prefix)
                           select new BoardProjection
                           {
                               Name = b.Name
                           }).Distinct().ToArray());
        }

        public IEnumerable<BoardProjection> GetBoardsByClientId(int ClientId)
        {
            return _repository.Project<Board, BoardProjection[]>(
                 boards => (from b in boards
                            where b.ClientId==ClientId
                            select new BoardProjection
                            {
                                BoardId = b.BoardId,
                                Name = b.Name
                            }).ToArray());

        }

        public CMSResult Save(Board newBoard)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Board, bool>(boards => (
                                from b in boards
                                where b.Name == newBoard.Name && b.ClientId == newBoard.ClientId
                                select b
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Board '{0}' already exists!", newBoard.Name) });
            }
            else
            {
                _repository.Add(newBoard);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Board '{0}' successfully added!", newBoard.Name) });
            }
            return result;
        }

        public CMSResult Update(Board oldBoard)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Board, bool>(boards => (from b in boards where b.BoardId != oldBoard.BoardId && b.Name == oldBoard.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Board '{0}' already exists!", oldBoard.Name) });
            }
            else
            {
                var board = _repository.Load<Board>(b => b.BoardId == oldBoard.BoardId);
                board.Name = oldBoard.Name;
                _repository.Update(board);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Board '{0}' successfully updated!", oldBoard.Name) });
            }
            return result;
        }

        public IEnumerable<BoardGridModel> GetBoardData(out int totalRecords, string Name,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Board, IQueryable<BoardGridModel>>(Boards => (
                 from b in Boards
                 select new BoardGridModel
                 {
                     BoardId = b.BoardId,
                     BoardName = b.Name,
                     CreatedOn = b.CreatedOn,

                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.BoardName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BoardGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }
            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }
            return query.ToList();
        }

        public IEnumerable<BoardGridModel> GetBoardDataByClientId(out int totalRecords, string Name, int userId,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int ClientId = userId;
            var query = _repository.Project<Board, IQueryable<BoardGridModel>>(Branches => (
                 from b in Branches

                 select new BoardGridModel
                 {
                     UserId = b.UserId,
                     ClientId = b.ClientId,
                     BoardId = b.BoardId,
                     BoardName = b.Name,
                     CreatedOn = b.CreatedOn,

                 })).AsQueryable();
            if (ClientId != 0)
            {
                query = query.Where(p => p.ClientId == ClientId);
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.BoardName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BoardGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }
            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }
            return query.ToList();
        }

    }
}
