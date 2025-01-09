using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;
using UniversityLMS.Infrastructure.DbContexts;

namespace UniversityLMS.Infrastructure.Repositories
{
    public class OutboxMessageRepository(AppDbContext dbContext, IBlobStorageService blobStorageService) : IOutboxMessageRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IBlobStorageService _blobStorageService = blobStorageService;


        public async Task<Guid> AddSubmission(OutboxMessage outboxMessage)
        {
            try
            {
                await _dbContext.OutboxMessages.AddAsync(outboxMessage);
                await _dbContext.SaveChangesAsync();    

                return outboxMessage.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

        }

        public async Task OutboxProcessor()
        {
            List<OutboxMessage> messages = await _dbContext.OutboxMessages.Where(m => !m.IsProcessed)
                                                                            .Take(10)
                                                                            .ToListAsync();



            foreach (OutboxMessage item in messages)
            {
                try
                {
                    bool res = await _blobStorageService.PushToQueue(item);

                    if (res)
                    {
                        item.IsProcessed = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to enqueue message: {ex.Message}");
                }
            }
            await _dbContext.SaveChangesAsync();
        }

    }
}
