# One-to-many relations

In a relational database you implement a one-to-many relation between two entities/tables using a primary key of the first entity/table and a foreign key of the second entity/table. In object oriented programming code, you typically implement a one-to-many relation with a collection of object references in the first entity/class and an object reference in the second entity/class.

Using Starcounter we recommend that you model a one-to-many relation as in a relational database, i.e. an object reference in the second entity/class, but also add a property (or method) in the first entity/class that returns a collection of object references. In this way you both support the relational model and the typical object oriented implementation at the same time.

In the code example below there is a one-to-many relation between the entities/classes <code>Department</code> and <code>Employee</code> regarding employment. This one-to-many relation is stored in the object references <code>Department</code> in the entity/class <code>Employee</code>.

```cs
[Database]
public class Department {
  ...
  public IEnumerable Employees {
    get {
      return Db.SQL<Employee>("select e from Employee e where e.Department = ?", this);
    }
  }
}

[Database]
public class Employee {
  ...
  public Department Department;
}
```

In the code above, all instances of the <code>Employee</code> class has a reference (<code>Department</code>) to the department where they are employed, and all instances of the <code>Department</code> class has a collection (<code>Employees</code>) of all employees of that particular department. However, within a <code>Department</code> object the collection is not stored internally in any data structure, but instead is represented by an SQL query.
