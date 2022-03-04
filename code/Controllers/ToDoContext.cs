/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * SPDX-License-Identifier: MIT-0
 */
 
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace todo_app.Controllers
{
    public class ToDoContext : DbContext
    {
        string TODO_CONNECTION_STRING = "";
        public DbSet<ToDo> ToDos { get; set; }
        public ToDoContext(IOptions<Parameters> options)
        {
            TODO_CONNECTION_STRING = options.Value.AuroraConnectionString;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(TODO_CONNECTION_STRING);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ToDo>(entity =>
            {
                entity.HasKey(e => e.CreatedTime);
                entity.Property(e => e.Task).IsRequired();
            });

        }
    }
}