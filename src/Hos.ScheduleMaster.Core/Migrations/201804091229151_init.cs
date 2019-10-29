namespace Hos.ScheduleMaster.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OptionConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false, maxLength: 1000),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ServerNodes",
                c => new
                    {
                        NodeName = c.String(nullable: false, maxLength: 128),
                        Host = c.String(nullable: false),
                        LastUpdateTime = c.DateTime(),
                        Status = c.Int(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NodeName);
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Contents = c.String(nullable: false),
                        TaskId = c.Guid(),
                        Node = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        Remark = c.String(maxLength: 500),
                        RunMoreTimes = c.Boolean(nullable: false),
                        CronExpression = c.String(maxLength: 50),
                        AssemblyName = c.String(nullable: false, maxLength: 200),
                        ClassName = c.String(nullable: false, maxLength: 200),
                        CustomParamsJson = c.String(maxLength: 500),
                        Status = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CreateTime = c.DateTime(nullable: false),
                        LastRunTime = c.DateTime(),
                        NextRunTime = c.DateTime(),
                        TotalRunCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaskLocks",
                c => new
                    {
                        TaskId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TaskLocks");
            DropTable("dbo.Tasks");
            DropTable("dbo.SystemLogs");
            DropTable("dbo.ServerNodes");
            DropTable("dbo.OptionConfigs");
        }
    }
}
