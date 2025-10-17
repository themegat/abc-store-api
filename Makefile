.PHONY: migrate-add migrate-apply

# Target to add a new migration
# Usage: make migrate-add NAME=MyNewMigration
migrate-add:
	@echo "Adding migration: $(NAME)"
	dotnet ef migrations add $(NAME)

# Target to apply migrations
# Usage: make migrate-apply
migrate-apply:
	@echo "Applying pending migrations..."
	dotnet ef database update --project