using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Converter coluna Role para integer apenas se ainda for text
            // Mapeamento: 'Admin' -> 1, 'Common' -> 2 (conforme enum UserRole)
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' AND column_name = 'Role' AND data_type = 'text'
                    ) THEN
                        ALTER TABLE ""Users"" 
                        ALTER COLUMN ""Role"" TYPE integer 
                        USING CASE 
                            WHEN ""Role"" = 'Admin' THEN 1
                            WHEN ""Role"" = 'Common' THEN 2
                            ELSE 2
                        END;
                    END IF;
                END $$;
            ");

            // Adicionar AvatarChoice apenas se não existir
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' AND column_name = 'AvatarChoice'
                    ) THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""AvatarChoice"" integer NOT NULL DEFAULT 0;
                    END IF;
                END $$;
            ");

            // Adicionar FirstName apenas se não existir
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' AND column_name = 'FirstName'
                    ) THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""FirstName"" character varying(50) NOT NULL DEFAULT '';
                    END IF;
                END $$;
            ");

            // Adicionar HasCompletedFirstPurchaseReview apenas se não existir
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' AND column_name = 'HasCompletedFirstPurchaseReview'
                    ) THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""HasCompletedFirstPurchaseReview"" boolean NOT NULL DEFAULT false;
                    END IF;
                END $$;
            ");

            // Adicionar LastName apenas se não existir
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Users' AND column_name = 'LastName'
                    ) THEN
                        ALTER TABLE ""Users"" ADD COLUMN ""LastName"" character varying(50) NOT NULL DEFAULT '';
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarChoice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HasCompletedFirstPurchaseReview",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            // Converter Role de volta para text
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ALTER COLUMN ""Role"" TYPE text 
                USING CASE 
                    WHEN ""Role"" = 1 THEN 'Admin'
                    WHEN ""Role"" = 2 THEN 'Common'
                    ELSE 'Common'
                END;
            ");
        }
    }
}


