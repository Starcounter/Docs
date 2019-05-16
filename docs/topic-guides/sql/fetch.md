# Fetch

## Introduction

The number of returned results can be limited with the `FETCH` clause:

```sql
SELECT e.LastName, e.FirstName
  FROM Employee e
  FETCH FIRST 5 ROWS ONLY
```

The only mandatory reserved word is the word `FETCH` as in the example below; the other reserved words are optional.

```sql
SELECT e.LastName, e.FirstName
  FROM Employee e
  FETCH 5
```

## Using FETCH with other clauses

The `FETCH` clause should be after the main part of the query possibly including an `ORDER BY` clause but before an `OPTION` clause including hints, see example below.

```sql
SELECT e.LastName, e.FirstName
  FROM Employee e
  ORDER BY e.FirstName
  FETCH 5
  OPTION INDEX (e MyIndexOnFirstName)
```

### FETCH with OFFSET

The `OFFSET` clause can be used to skip a number of rows before beginning to fetch the rows. This can be used to apply patterns like result pagination. `OFFSET 0` is the same as omitting the `OFFSET` clause.

```sql
SELECT e.LastName, e.FirstName
  FROM Employee e
  FETCH 5
  OFFSET 50
```

The standard `OFFSET` functionality typically used in RESTful web applications has a set of issues when the database is updated while data is being fetched. When using the standard `OFFSET` and data is updated, deleted or inserted between requests, the client will receive the same row twice \(if a row already retrieved was inserted\) or miss a row \(if a row already retrieved was deleted\).

Furthermore, `OFFSET` has performance limitations, since it is difficult to know which objects should be retrieved from each table to skip the requested number of rows in the result.

