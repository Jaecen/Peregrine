namespace Peregrine.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Tournament_Organizers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Organizer_Tournament",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TournamentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.TournamentId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TournamentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Organizer_Tournament", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Organizer_Tournament", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Organizer_Tournament", new[] { "TournamentId" });
            DropIndex("dbo.Organizer_Tournament", new[] { "UserId" });
            DropTable("dbo.Organizer_Tournament");
        }
    }
}
