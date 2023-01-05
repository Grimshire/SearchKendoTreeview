using System.Collections.Generic;
using System.Linq;

namespace SearchKendoTreeview
{
    public class TreeHelper
    {
        /*
            For searching Kendo Trees with a text query, the entry point is FindMatchingNodes(model, queryTxt)
            model = the tree you want to search
            queryTxt = the string you want to find
         */
        public class KendoTreeModel
        {
            public int id { get; set; }
            public int parentId { get; set; }            
            public string shortName { get; set; }
            public string longName { get; set; }
            public List<KendoTreeModel> items { get; set; }
        }

        private static List<KendoTreeModel> GetParentNodes(List<KendoTreeModel> flattenedList, KendoTreeModel node)
        {
            KendoTreeModel parentNode = flattenedList.Where(x => x.id == node.parentId).FirstOrDefault();
            List<KendoTreeModel> parents = new List<KendoTreeModel>();
            parents.Add(parentNode);
            if (parentNode != null)
            {
                parents.AddRange(GetParentNodes(flattenedList, parentNode));
            }
            return parents;
        }

        public static List<KendoTreeModel> FlattenList(KendoTreeModel tree)
        {
            List<KendoTreeModel> flattenedList = new List<KendoTreeModel>();
            flattenedList.AddRange(tree.items);

            //items2addWhileIterating 
            List<KendoTreeModel> newItems = new List<KendoTreeModel>();
            foreach (var X in flattenedList)
            {
                if (X.items != null && X.items.Count > 0)
                {
                    newItems.AddRange(FlattenList(X));
                }
            }

            //don't forget to add the root node back to the front of the list
            flattenedList.AddRange(newItems);

            return flattenedList;
        }

        public static List<KendoTreeModel> FindMatchingNodes<T>(List<T> model, string queryTxt)
            where T : KendoTreeModel
        {

            //this flattens the model
            List<KendoTreeModel> flattenedList = new List<KendoTreeModel>();
            flattenedList = FlattenList(model[0]);
            //ensure no duplicates
            flattenedList = flattenedList.GroupBy(m => m.id).Select(y => y.First()).ToList();

            string upperQUERYTXT = queryTxt.ToUpper();
            //get only the nodes that match the query (the deepest children, the leaf on the branch if you will)
            var leaves = flattenedList.Where(x => x.longName.ToUpper().IndexOf(upperQUERYTXT) > -1 || x.shortName.ToUpper().IndexOf(upperQUERYTXT) > -1).ToList();

            //Clean parent of all nodes that do not belong to the paths            
            foreach (var node in flattenedList)
            {
                node.items = null;
                node.items = new List<KendoTreeModel>();
            }

            //unflatten BUT only by using acceptable members of the branches and the leaves themselves                        
            List<KendoTreeModel> list2return = new List<KendoTreeModel>();
            foreach (var X in leaves)
            {
                List<KendoTreeModel> nodes2add = GetParentNodes(flattenedList, X);
                if (nodes2add != null)
                {
                    list2return.AddRange(nodes2add);
                }
                list2return.Add(X);
            }

            //get rid of nulls and duplicates
            list2return = list2return.Where(x => x != null).GroupBy(x => x.id).Select(x => x.FirstOrDefault()).ToList();
            //rebuild the tree
            //return just the root with that root's items (children) and the items of that one, etc.
            KendoTreeModel root = model[0];
            root.items = new List<KendoTreeModel>();
            root = BuildTree(root, list2return);

            //Kendo datasources like arrays and not objects
            list2return = new List<KendoTreeModel>();
            list2return.Add(root);

            return list2return;
        }

        public static KendoTreeModel BuildTree(KendoTreeModel root, List<KendoTreeModel> nodes)
        {
            if (nodes.Count == 0) return root;
            var children = nodes.Where(n => n.parentId == root.id);
            root.items.AddRange(children);

            for (int i = 0; i < root.items.Count; i++)
            {
                root.items[i] = BuildTree(root.items[i], nodes);
                if (nodes.Count == 0) { break; }
            }

            return root;
        }
    }
}