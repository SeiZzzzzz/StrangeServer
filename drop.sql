USE X
    EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
    EXEC sp_MSForEachTable 'DELETE FROM ?'

    DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR
    SET @Cursor = CURSOR FAST_FORWARD FOR

    SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + ']'
    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
    LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME
    OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql
    WHILE (@@FETCH_STATUS = 0)
      BEGIN
        Exec SP_EXECUTESQL @Sql
        FETCH NEXT 
        FROM @Cursor INTO @Sql
      END
    CLOSE @Cursor DEALLOCATE @Cursor
    GO

    EXEC sp_MSForEachTable 'DROP TABLE ?'
    GO

    EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'