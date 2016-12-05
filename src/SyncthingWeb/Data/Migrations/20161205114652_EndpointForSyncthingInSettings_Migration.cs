using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SyncthingWeb.Data.Migrations
{
    public partial class EndpointForSyncthingInSettings_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(name: "SyncthingConfigPath", table: "GeneralSettingses");

            migrationBuilder.AddColumn<string>(
                name: "SyncthingEndpoint",
                table: "GeneralSettingses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncthingApiKey",
                table: "GeneralSettingses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "SyncthingEndpoint",
            //    table: "GeneralSettingses");

            //migrationBuilder.DropColumn(
            //    name: "SyncthingApiKey",
            //    table: "GeneralSettingses");

            //migrationBuilder.AddColumn<string>(
            //    name: "SyncthingConfigPath",
            //    table: "GeneralSettingses",
            //    nullable: true);
        }
    }
}
