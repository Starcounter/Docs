Let's deepen into technical details in order to understand how two layers, application and database, are physically merged into one.</span>

<span style="color: #000000;">We can say that database and application server are represented by two parallel running processes. In the picture they are presented as ScDATA and ScCODE respectively. Inbound and outbound traffic towards those processes is initially handled by Gateway process.</span><img alt="" src="http://starcounter.io/wp-content/uploads/2016/06/web-DB-explanation.gif" />

[web-DB-explaination](../Docs/images/web-DB-explaination.gif)

Distinction of depicted processes:
<ul>
	<li><strong>ScNETWORKGATEWAY </strong>represents a Gateway for network traffic, delivering requests to execute and sending responses back (read more <a href="http://starcounter.io/guides/network/networkgateway/">here</a>);</li>
	<li><strong>ScCODE</strong> represents Codehost as a run-space for all the applications operating on the same Database;</li>
	<li><strong>ScDATA</strong> represents Database and manages database memory handling for Codehost activity.</li>
</ul>
Communication between processes is organized through the shared memory. This allows processes to efficiently exchange messages, preserving processes isolation, security and consistency.
<h3>Codehost isolation</h3>
Gateway and Codehost processes operate on one shared memory segment, while Database and Codehost on another, meaning that Gateway has no direct access to Database process - it can only communicate with the Codehost.

This is done for multiple reasons:
<ol>
	<li>Failure/restart of the Codehost process does not affect the Database.</li>
	<li>Developers can iterate application versions and update those without Database process restart (only Codehost is restarted and being reconnected to Database);</li>
	<li>Database is isolated from networking process therefore it is impossible to affect the Database directly through Gateway.</li>
	<li>In future each application will have their own Codehost processes to ensure app independence and overall system stability.</li>
</ol>[/content_box][/content_boxes][/fullwidth]