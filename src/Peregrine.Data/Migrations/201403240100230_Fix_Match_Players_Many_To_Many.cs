namespace Peregrine.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fix_Match_Players_Many_To_Many : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Players", "Match_Id", "dbo.Matches");
            DropIndex("dbo.Players", new[] { "Match_Id" });
            CreateTable(
                "dbo.Match_Players",
                c => new
                    {
                        MatchId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MatchId, t.PlayerId })
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.MatchId)
                .Index(t => t.PlayerId);
            
            DropColumn("dbo.Players", "Match_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "Match_Id", c => c.Int());
            DropForeignKey("dbo.Match_Players", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.Match_Players", "MatchId", "dbo.Matches");
            DropIndex("dbo.Match_Players", new[] { "PlayerId" });
            DropIndex("dbo.Match_Players", new[] { "MatchId" });
            DropTable("dbo.Match_Players");
            CreateIndex("dbo.Players", "Match_Id");
            AddForeignKey("dbo.Players", "Match_Id", "dbo.Matches", "Id");
        }
    }
}
