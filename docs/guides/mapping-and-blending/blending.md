# Blending

A launcher can implement _blending_. It is a feature of rearranging the rendering of the HTML response.

By default, the merged HTML response from multiple applications comes with the application starting order. Where the output of a first application finishes, the output of a second application begins. This is far from the desired situation.

Normally, you want to achieve a particular rendering order. You can use CSS for that. There is myriad ways: absolute positioning, flexbox and CSS Grid Layout. All of these solutions require applying arbitrary CSS on top of the running app, which is not very flexible.

To replace this tedious process, a Launcher might implement a layout editor. Our reference Launcher has a layout editor that can be invoked by pressing CTRL+E or the paint icon. It's a generator for CSS Grid Layout with a shim that uses HTML `<table>` and Shadow DOM.

