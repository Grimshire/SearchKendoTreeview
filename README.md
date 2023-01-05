# SearchKendoTreeview
Nuget package for SearchKendoTreeview. Search Kendo Treeviews with a text query. The results will be any matching nodes and all of their parent nodes which effectively yields branches that match the query.

# How to Use
The entrypoint, or method most users will be interested in is found at SearchKendoTreeview.TreeHelper.FindMatchingNodes(model, queryTxt).

**Parameters:**
model = the tree you want to search
queryTxt = the string you want to find

You will need to create a model that inherits from SearchKendoTreeview and can be passed to this method like so:

![Example for how to inherit from KendoTreeModel](https://github.com/Grimshire/SearchKendoTreeview/blob/main/SearchKendoTreeview.png)

Finally, an example for how to get the model back:

![Example for how to return a tree in an MVC controller method](https://github.com/Grimshire/SearchKendoTreeview/blob/main/SearchKendoTreeview2.png)
