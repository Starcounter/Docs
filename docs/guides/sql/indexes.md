# Indexes

## Introduction

Indexes are essential to reach optimal performance. `CREATE INDEX` creates indexes and `DROP INDEX` removes indexes.

## Declaring indexes

Indexes are created with `Db.SQL`, where query string contains an index declaration and has the form `CREATE [UNIQUE] INDEX indexName ON typeName (propertyName [ASC/DESC], ...)`.

It is recommended to declare indexes before any retrieval query is issued. **Note** that indexes should be declared **outside** a transaction scope.

In the examples \(1\), \(2\), \(3\), \(4\) and \(5\) below we declare indexes on different properties/columns of the class/table Employee.

```sql
(1) CREATE INDEX EmpFirstNameIndex ON Employee (FirstName ASC)

(2) CREATE INDEX EmpLastNameIndex ON Employee (LastName ASC)

(3) CREATE INDEX EmpSalaryIndex ON Employee (Salary)

(4) CREATE INDEX EmpManagerIndex ON Employee (Manager)

(5) CREATE INDEX EmpDepartmentIndex ON Employee (Department)
```

The default order, used when there is no order declared, is ASC \(ascending\). The order specified in the index declaration only matters when you want the result set to be sorted \(ORDER BY\).

You can declare indexes on properties/columns of the following datatypes \(DbTypeCode\): Boolean, Byte, DateTime, Decimal, Int16, Int32, Int64, Object, SByte, String, UInt16, UInt32, UInt64.

You can declare combined indexes on up to ten different properties/columns of a class/table. In the examples \(6\), \(7\), \(8\) and \(9\) we have some combined indexes on two properties/columns of the class/table Employee.

```sql
(6) CREATE INDEX EmpLastnameFirstNameIndex ON Employee (LastName ASC, FirstName ASC)

(7) CREATE INDEX EmpFirstNameLastNameIndex1 ON Employee (FirstName ASC, LastName ASC)

(8) CREATE INDEX EmpFirstNameLastNameIndex2 ON Employee (FirstName DESC, LastName ASC)

(9) CREATE INDEX EmpDepartmentSalaryIndex ON Employee (Department ASC, Salary DESC)
```

## Checking for declared indexes

The `indexName` must be unique. If you define the same name more than once you will get an exception. It is possible to check if an index was already created by issuing a query, which selects a record from table `Starcounter.Metadata.Index` with column `Name` equivalent to the index name as in the example below.

```csharp
if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "EmpDepartmentSalaryIndex").FirstOrDefault() == null)
    Db.SQL("CREATE INDEX EmpDepartmentSalaryIndex ON Employee (Department ASC, Salary DESC)");
```

## Dropping indexes

Existing indexes can be dropped from a database by query with syntax `DROP INDEX index_name ON table_name`. For example:

```csharp
Db.SQL("DROP INDEX EmpDepartmentSalaryIndex ON Employee");
```

## Recommendations

For all SELECT statements in your programming code, it is recommended to declare, when possible, indexes for:

* all conditions in WHERE clauses,
* all join conditions,
* all sort specifications in ORDER BY clauses.

An execution of the query \(11\) below can make use of an index on the property/column FirstName such as the index \(1\) above. It can also make use of combined indexes such as \(7\) or \(8\) where the first property/column of the index is FirstName. It can not make use of the combined index \(6\) where FirstName is not the first property/column of the index.

An execution of the query \(12\), where we have two equality comparisons on FirstName and LastName, can make use of any of the combined indexes \(6\), \(7\) and \(8\), where the two first properties/columns of the indexes are FirstName and LastName.

An execution of the query \(13\) can efficiently make use of the combined indexes \(7\) and \(8\) because the first property/column of the index is FirstName, on which we have an equality comparison, and the second property/column of the index is LastName, on which we have a range comparison. However, an execution of this query can not efficiently make use of the combined index \(6\) because the range condition on the first property/column LastName of the index makes it not possible to use the equality condition on the subsequent property/column FirstName.

An execution of the join in the query \(14\) can make use of the indexes \(5\) or \(9\), since we need an index to efficiently find all Employee objects/rows for a particular Department object/row.

An execution of the query \(15\) can make use of the combined index \(6\) to find all Employee objects/rows in the requested order without any sorting.

An execution of the query \(16\) can also make use of the combined index \(6\) to find all Employee objects/rows in the requested order without any sorting, since the index can be traversed in the reverse order.

An execution of the query \(17\) can make use of the combined index \(7\) in the reverse order to find all Employees objects/rows in the requested order without any sorting order. However, an execution of this query can not efficienlty make use of the combined index \(8\) since it neither in the normal nor the reverse order match the requested order.

```sql
(11) SELECT e FROM Employee e WHERE e.FirstName = ?

(12) SELECT e FROM Employee e WHERE e.FirstName = ? AND e.LastName = ?

(13) SELECT e FROM Employee e WHERE e.FirstName = ? AND e.LastName > ?

(14) SELECT e, d FROM Employee e JOIN Department d
         ON e.Department = d WHERE d.Name = ?

(15) SELECT e FROM Employee e ORDER BY e.LastName ASC, e.FirstName ASC

(16) SELECT e FROM Employee e ORDER BY e.LastName DESC, e.FirstName DESC

(17) SELECT e FROM Employee e ORDER BY e.FirstName DESC, e.LastName DESC
```

## Index hints in queries

You can give a hint to Starcounter on what index to use for a specific query. See [Hints](query-plan-hints.md) for more information.

## Derived indexes

The current version do not support derived indexes. You need to define index on the class you like to query. For instance, say we have the following structure:

```csharp
[Database]
public class LegalEntity
{
   public string Name { get; set; }
}

public class Company : LegalEntity
{}

public class Person : LegalEntity
{}
```

You will need to define index on Name for both Company and Person.

If index is defined on a database property for the base class, the query optimizer might choose to use it in queries on a child class of the base class, but this will require to filter out all instances, which are not of the child class. For example, index is created on `Name` only for `LegalEntity` and a query is submitted for `Company`, then if the query optimizer chooses to use the index, it will add a filter predicate, which checks that all objects from the index are instances of `Company`.

