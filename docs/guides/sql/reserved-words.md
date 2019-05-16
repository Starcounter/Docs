# Reserved words

The following words are reserved words in Starcounter SQL.

`ALL`, `AND`, `AS`, `ASC`, `AVG`,  
`BY`, `BINARY`,  
`CAST`, `COUNT`, `CREATE`, `CROSS`,  
`DATE`, `DATETIME`, `DELETE`, `DESC`, `DISTINCT`,  
`ESCAPE`, `EXISTS`,  
`FALSE`, `FETCH`, `FIRST`, `FIXED`, `FORALL`, `FROM`, `FULL`,  
`GROUP`,  
`HAVING`,  
`IN`, `INDEX`, `INNER`, `INSERT`, `IS`,  
`JOIN`,  
`LEFT`, `LIKE`, `LIMIT`,  
`MAX`, `MIN`,  
`NOT`, `NULL`,  
`OBJ`, `OBJECT`, `OFFSET`, `OFFSETKEY`, `ON`, `ONLY`,  
`OPTION`, `OR`, `ORDER`, `OUT`, `OUTER`, `OUTPUT`,  
`PROC`, `PROCEDURE`,  
`RANDOM`, `RIGHT`, `ROWS`,  
`SELECT`, `STARTS`, `SUM`,  
`TIME`, `TIMESTAMP`, `TRUE`,  
`UNIQUE`, `UNKNOWN`, `UPDATE`,  
`VALUES`, `VAR`, `VARIABLE`,  
`WHEN`, `WHERE`, `WITH`.

Be aware that the list of reserved words might be extended in later versions of Starcounter SQL. In particular some keywords in SQL92 might become reserved words in Starcounter SQL.

Reserved words cannot be used in queries directly. They have to be surrounded with double quotes as in example:

```
SELECT d FROM "DATE" d
SELECT o FROM "ORDER" o
```

Double quoting can be applied to any identifier, but only necessary for reserved keywords. It is important to double quote each identifier in identifier change, e.g.:

```
SELECT t FROM "Order"."Date" t
```

