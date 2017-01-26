# Datatypes

This page describes the expected datatypes of the `Db.SQL` query results for the database object properties, fields and arithmetic operations.

## Properties and fields

Your object properties and fields may have the following datatypes (<code>DbTypeCode</code>):

<ul>
<li><code>Binary</code>, </li>
<li><code>Boolean</code>, </li>
<li><code>Byte</code>, </li>
<li><code>DateTime</code>, </li>
<li><code>Decimal</code>, </li>
<li><code>Double</code>, </li>
<li><code>Int16</code>, </li>
<li><code>Int32</code>, </li>
<li><code>Int64</code>, </li>
<li><code>object</code> (reference to a database object), </li>
<li><code>SByte</code>, </li>
<li><code>Single</code>, </li>
<li><code>String</code>, </li>
<li><code>UInt16</code>, </li>
<li><code>UInt32</code>, </li>
<li><code>UInt64</code>.</li>
</ul>

<p>The datatypes <code>Boolean</code>, <code>Byte</code>, <code>DateTime</code>, <code>Decimal</code>, <code>Double</code>, <code>Int16</code>, <code>Int32</code>, <code>Int64</code>, <code>SByte</code>, <code>Single</code>, <code>String</code>, <code>UInt16</code>, <code>UInt32</code>, <code>UInt64</code> correspond to the .NET datatypes with the same names.

The datatype <code>object</code> represents a reference to a database object, i.e. an instance of a class, directly or by inheritance having the<code>Database</code> attribute set.

The datatype <code>Binary</code> is for representing binary data up to 8 kB. Note that in Starcounter there is also another binary datatype <code>LargeBinary</code> for storing larger binary data. However, <code>LargeBinary</code> cannot be indexed and is not supported in Starcounter SQL.</p>

If you want to store <code>null</code> values for datatypes that essentially are value types, you can instead use the corresponding nullable datatypes:

[](* `Nullable&lt;Binary&gt;`,)
* <code>Nullable&lt;Boolean&gt;</code>,
* <code>Nullable&lt;Byte&gt;</code>,
* <code>Nullable&lt;DateTime&gt;</code>,
* <code>Nullable&lt;Decimal&gt;</code>,
* <code>Nullable&lt;Double&gt;</code>,
* <code>Nullable&lt;Int16&gt;</code>,
* <code>Nullable&lt;Int32&gt;</code>,
* <code>Nullable&lt;Int64&gt;</code>,
* <code>Nullable&lt;SByte&gt;</code>,
* <code>Nullable&lt;Single&gt;</code>,
* <code>Nullable&lt;UInt16&gt;</code>,
* <code>Nullable&lt;UInt32&gt;</code>,
* <code>Nullable&lt;UInt64&gt;</code>.

[](<p>Internally, in Starcounter SQL, all signed integers <code>Int64</code>, <code>Int32</code>,
<code>Int16</code>, <code>SByte</code> are represented as <code>Int64</code>, all unsigned integers
<code>UInt64</code>, <code>UInt32</code>, <code>UInt16</code>, <code>Byte</code> are
represented as <code>UInt64</code>, and all approximate numerical types <code>Single</code>,
<code>Double</code> are represented as <code>Double</code>.</p>)

## Arithmetic operations

The datatype of the result of an [arithmetic operation](/guides/SQL/data-operators/) is one of the following:

1. <code>Double</code> (representing approximate numeric values) [highest precedence],
2. <code>Decimal</code> (representing exact numeric values),
3. <code>Int64</code> (representing signed integers),
4. <code>UInt64</code> (representing unsigned integers - the natural numbers) [lowest precedence]

In general the datatype of the result of an arithmetic operation is the datatype with the highest precedence of the datatypes of the operands.

However, in the following special cases you need a datatype with higher precedence to appropriately represent the result:

- A subtraction between <code>UInt64</code>'s (unsigned integers) has a result of datatype <code>Int64</code> (signed integer).
- A division between any combination of <code>UInt64</code>'s and <code>Int64</code>'s (unsigned and signed integers) has a result of datatype <code>Decimal</code>
