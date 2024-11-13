import matplotlib.pyplot as plt
import networkx as nx
import pandas as pd


class GraphVisualizer:
    def __init__(self, graphs):
        self.graphs = graphs
        self.current_graph_index = 0
        self.fig, self.ax = plt.subplots()
        self.nodes = None
        self.pos = None
        self.nodelist = None
        self.tooltips = {}
        self.draw_graph()
        self.connect_events()

    def generate_node_layers(self, graph):
        # Initialize node layers for the multipartite layout
        node_layers = {}
        roots = [node for node in graph.nodes() if graph.in_degree(node) == 0]

        # Use BFS to assign layers to each node
        from collections import deque
        queue = deque()
        for root in roots:
            node_layers[root] = 0
            queue.append(root)

        while queue:
            current_node = queue.popleft()
            current_layer = node_layers[current_node]
            for child in graph.successors(current_node):
                if child not in node_layers or node_layers[child] < current_layer + 1:
                    node_layers[child] = current_layer + 1
                    queue.append(child)

        return node_layers

    def draw_graph(self):
        self.ax.clear()
        graph = self.graphs[self.current_graph_index]

        # Generate node layers for layout
        node_layers = self.generate_node_layers(graph)
        nx.set_node_attributes(graph, node_layers, name="subset")

        # Position nodes using multipartite layout
        self.pos = nx.multipartite_layout(graph, subset_key="subset")
        self.nodelist = list(graph.nodes())

        # Draw nodes and edges
        self.nodes = nx.draw_networkx_nodes(
            graph, self.pos, ax=self.ax, node_color="lightblue", nodelist=self.nodelist
        )
        nx.draw_networkx_edges(graph, self.pos, ax=self.ax, edge_color="gray")

        self.fig.canvas.draw_idle()

    def connect_events(self):
        self.hover_cid = self.fig.canvas.mpl_connect("motion_notify_event", self.on_hover)
        self.key_cid = self.fig.canvas.mpl_connect("key_press_event", self.on_key)
        self.resize_cid = self.fig.canvas.mpl_connect("resize_event", self.on_resize)

    def on_hover(self, event):
        graph = self.graphs[self.current_graph_index]

        if self.tooltips.get(self.current_graph_index):
            tooltip = self.tooltips[self.current_graph_index]
        else:
            tooltip = self.ax.annotate(
                "",
                xy=(0, 0),
                xytext=(10, 10),
                textcoords="offset points",
                color="w",
                ha="center",
                fontsize=8,
                bbox=dict(
                    boxstyle="round,pad=0.5",
                    fc=(0.1, 0.1, 0.1, 0.9),
                    ec="white",
                    lw=1,
                ),
            )
            self.tooltips[self.current_graph_index] = tooltip
        tooltip.set_visible(False)

        if event.inaxes == self.ax:
            cont, ind = self.nodes.contains(event)
            if cont:
                idx = ind["ind"][0]
                node = self.nodelist[idx]
                node_data = graph.nodes[node]

                # Position tooltip directly over the node
                tooltip.xy = self.pos[node]
                tooltip.set_text(
                    f"Hash: {node_data.get('original_id', 'Unknown')}\n"
                    f"Type: {node_data.get('Type', 'Unknown')}\n"
                    f"Move: {node_data.get('Applied Move', 'Unknown')}\n"
                    f"======\n"
                    f"Visit: {node_data.get('Visit', 'Unknown')}\n"
                    f"Score: {node_data.get('Score', 'Unknown')}\n"
                    f"======\n"
                    f"Coins: {node_data.get('Coins', 'Unknown')}\n"
                    f"Power: {node_data.get('Power', 'Unknown')}\n"
                    f"Prestige: {node_data.get('Prestige', 'Unknown')}"
                )
                tooltip.set_visible(True)
                self.fig.canvas.draw_idle()
            else:
                tooltip.set_visible(False)
                self.fig.canvas.draw_idle()

    def on_key(self, event):
        if event.key == "right":
            self.current_graph_index = (self.current_graph_index + 1) % len(self.graphs)
            self.update_graph()
        elif event.key == "left":
            self.current_graph_index = (self.current_graph_index - 1) % len(self.graphs)
            self.update_graph()

    def on_resize(self, event):
        self.update_graph()

    def update_graph(self):
        # Disconnect the old hover event
        self.fig.canvas.mpl_disconnect(self.hover_cid)
        self.draw_graph()
        # Reconnect the hover event with the updated nodes
        self.hover_cid = self.fig.canvas.mpl_connect("motion_notify_event", self.on_hover)
        self.fig.canvas.draw_idle()


def generate_graphs(data):
    graphs = []

    # Extract unique node IDs from 'Node' and 'Children' columns
    node_ids = set(data['Node'].astype(int).tolist())
    children_ids = data['Children'].dropna().apply(lambda x: [int(y) for y in x.split(';')])
    children_ids = set([child_id for sublist in children_ids for child_id in sublist])

    # Combine and sort node IDs for sequential mapping
    all_node_ids = sorted(list(node_ids.union(children_ids)))

    # Create a mapping from large node IDs to sequential integers
    node_id_to_seq_id = {node_id: idx for idx, node_id in enumerate(all_node_ids)}

    # Identify the indices of root nodes (where 'Applied Move' is NaN)
    root_indices = data[data["Applied Move"].isna()].index
    for i, root_i in enumerate(root_indices):
        start = root_i
        end = root_indices[i + 1] if i + 1 < len(root_indices) else len(data)
        segment = data.iloc[start:end]

        G = nx.DiGraph()

        for _, row in segment.iterrows():
            node_id = int(row["Node"])
            seq_node_id = node_id_to_seq_id[node_id]

            # Set node attributes and include the original node ID
            attributes = row.to_dict()
            attributes['original_id'] = node_id
            G.add_node(seq_node_id, **attributes)

            if pd.notna(row["Children"]):
                children_ids = [int(child_id) for child_id in str(row["Children"]).split(";")]
                for child_id in children_ids:
                    seq_child_id = node_id_to_seq_id[child_id]
                    G.add_edge(seq_node_id, seq_child_id)

        graphs.append(G)
    return graphs


def main():
    # Read the CSV data
    data = pd.read_csv("tree.csv")

    # Generate graphs from the data
    graphs = generate_graphs(data)

    # Create the visualizer and start the event loop
    visualizer = GraphVisualizer(graphs)
    plt.show()


if __name__ == "__main__":
    main()
