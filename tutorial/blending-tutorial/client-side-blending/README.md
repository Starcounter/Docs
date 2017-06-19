# Client-Side Blending

The only thing remaining now to have our medical system for pets is to make the different views good together. Right now, the views are simply stacked on top of each other. 

We use client-side blending to fix this. With the change to Declarative Shadow DOM, this is trivial. 

## CompositionEditor and CompositonProvider

[CompositionEditor](https://github.com/starcounter/compositionEditor) makes it possible to edit the result of server-side blending in the browser. Changes made by the CompositionEditor are saved in the database. 

[CompositionProvider](https://github.com/starcounter/compositionprovider) serves these edited views from the database. 

## Using the CompositionEditor

* Start MedicalRecordProvider and PetList
* Go to the warehouse: `http://localhost:8181/#/databases/default/appstore`
* Download and start CompositionProvider and CompositionEditor

Now, go to `http://localhost:8080/PetList`. If you click `Ctrl + E`, an editor will appear. 

Click on the dropdown and choose `[partial-id="/sc/htmlmerger?PetList=/PetList/views/PetListPage.html&MedicalRecordProvider=/MedicalRecordProvider/views/RecordSummary.html"]`

Reading this line closely, you see that it's the merged HTML of `PetListPage.html` and `RecordSummary.html`. The html for it should look like this:

```html 
<slot name="pet-list/list"></slot>
<slot name="medicalrecordprovider/summary-headline"></slot>
<slot name="medicalrecordprovider/summary-examination-number"></slot>
<slot name="medicalrecordprovider/summary-percentage"></slot>
<slot name="medicalrecordprovider/summary-most-visited"></slot>
<slot name="medicalrecordprovider/summary-latest"></slot>
```

### Blending the List

We want all the information from the summary to appear in a card on the bottom of the list. This can be done by wrapping them in a `div` and giving it the style of a card:

```html
<style>
	.summary-card {
		box-sizing: border-box;
		max-width: 500px;
		margin: 0 auto;
		padding: 20px;
		box-shadow: 0 1px 3px #999;
	}
</style>
<slot name="pet-list/list"></slot>
<div class="summary-card">
	<slot name="medicalrecordprovider/summary-headline"></slot>
	<slot name="medicalrecordprovider/summary-examination-number"></slot>
	<slot name="medicalrecordprovider/summary-percentage"></slot>
	<slot name="medicalrecordprovider/summary-most-visited"></slot>
	<slot name="medicalrecordprovider/summary-latest"></slot>
</div>
```

It looks like this:

![Final blending list](/assets/FinalBlendingList.PNG)

### Blending the Detail Pages

To blend the detail pages, go to `http://localhost:8080/PetList/Details?name=Lassie`.

There we see that the initial HTML composition looks like this:

```html
<style>
    @import '/PetList/style.css';
</style>
<div class="pet-list-wrapper">
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-name"></slot>
        <slot name="petlist/details-age-and-animal"></slot>
    </div>
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-owner-name"></slot>
        <slot name="petlist/details-weight"></slot>
    </div>
    <slot name="petlist/details-list-link"></slot>
</div>
<slot name="medicalrecordprovider/records-list-headline"></slot>
<slot name="medicalrecordprovider/records-list-table"></slot>
```

We want the list headline and table to be below the pet details but above the link. For this to happen, we just have to move them there. Additionally, the card is not wide enough to properly fit a table, so we will also increase its width:

```html
<style>
    @import '/PetList/style.css';
    .pet-list-wrapper {
    	max-width: 900px;
    }
</style>
<div class="pet-list-wrapper">
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-name"></slot>
        <slot name="petlist/details-age-and-animal"></slot>
    </div>
    <div class="pet-list-wrapper__row">
        <slot name="petlist/details-owner-name"></slot>
        <slot name="petlist/details-weight"></slot>
    </div>
    <slot name="medicalrecordprovider/records-list-headline"></slot>
    <slot name="medicalrecordprovider/records-list-table"></slot>
    <slot name="petlist/details-list-link"></slot>
</div>
```

![Final Blending Details](/assets/FinalBlendingDetails.PNG)

## Summary 

With this, we have taken two apps that did not previous know of each other and made them work as one.

![Final result](/assets/MedicalRecordProvider.gif)