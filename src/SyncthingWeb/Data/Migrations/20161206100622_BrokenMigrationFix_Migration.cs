using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyncthingWeb.Data.Migrations
{
    public partial class BrokenMigrationFix_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "SyncthignApiKey",
            //    table: "GeneralSettingses",
            //    newName: "SyncthingApiKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "SyncthingApiKey",
            //    table: "GeneralSettingses",
            //    newName: "SyncthignApiKey");
        }
    }
}
