﻿$principals:{principal |
$principal.databases:{database |
:connect $database.server$
use [$database.name$];
go

$current_user(principal.name, database, securables=database.securables)$

go
}$
}$
