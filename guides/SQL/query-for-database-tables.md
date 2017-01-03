# Query for Database Tables

Sometimes it may be helpful to get the tables in the database. That is possible to do using the class ClrClass.

For example, if the goal is to get all the tables in the database, a query like this would be used in the Starcounter Administrator which is found at `http://localhost:8181/#/databases/default/sql` when Starcounter is running:
```SQL
SELECT * FROM ClrClass
```

This would give back all the tables, including the built-in ones.

If the goal is to find all the user-created tables, it is possible to use the following query:
```SQL
SELECT *
FROM ClrClass c
WHERE c.Updatable=true
AND c.UniqueIdentifier NOT LIKE 'Simplified%'
AND c.UniqueIdentifier NOT LIKE 'Concepts.Ring%'
AND c.UniqueIdentifier NOT LIKE 'Starcounter.%'
AND c.UniqueIdentifier NOT LIKE 'SocietyObjects%'
```

For example, if the query is run after following the steps in<a href='https://starcounter.io/hello-world/create-a-database-class-hello-world-part-1/'> part one</a> of the HelloWorld tutorial, it should look like this:

<a href="https://starcounter.io/wp-content/uploads/2016/12/Capture.png" rel="attachment wp-att-17727"><img src="https://starcounter.io/wp-content/uploads/2016/12/Capture.png" alt="Query result" width="672" height="331" class="alignnone size-full wp-image-17727" /></a>