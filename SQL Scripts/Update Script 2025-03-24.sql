alter table [dbo].[AskBIHistory] Add [Status] tinyint null
go
update [dbo].[AskBIHistory] set [Status] = 2
go
alter table [dbo].[AskBIHistory] alter column [Status] tinyint not null
go
alter table [dbo].[CMOChatHistory] Add [Status] tinyint null
go
update [dbo].[CMOChatHistory] set [Status] = 2
go
alter table [dbo].[CMOChatHistory] alter column [Status] tinyint not null
go
alter table [dbo].[CMOChatHistory] Add [SessionId] uniqueidentifier null
go
update [dbo].[CMOChatHistory] set [SessionId] = UniqueId
go
alter table [dbo].[CMOChatHistory] alter column [SessionId] uniqueidentifier not null
go
alter table [dbo].[AskBIHistory] Add [SessionId] uniqueidentifier null
go
update [dbo].[AskBIHistory] set [SessionId] = UniqueId
go
alter table [dbo].[AskBIHistory] alter column [SessionId] uniqueidentifier not null
