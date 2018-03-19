# Reserved words

## Introduction

There are certain keywords in SQL that are marked as reserved. The reserved words have to be surrounded by double quotes when not meant as keywords.

## Reserved Words

These are the reserved words in Starcounter SQL in alphabetic order

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

The list of reserved words might be extended in later versions of Starcounter SQL. In particular some keywords in SQL92 might become reserved words in Starcounter SQL.

## Escaping Reserved Words

Reserved words cannot be used in queries directly. They have to be surrounded with double quotes as in example:

```sql
SELECT d FROM "DATE" d
SELECT o FROM "ORDER" o
```

Double quoting can be applied to any identifier, but only necessary for reserved keywords. It is important to double quote each identifier in identifier change, e.g.:

```sql
SELECT t FROM "Order"."Date" t
```

{% hint style="warning" %}
You can't use square brackets `[ ]` to escape reserved words in SQL
{% endhint %}

