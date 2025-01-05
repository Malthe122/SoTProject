import pandas as pd
import matplotlib.pyplot as plt
import os


def main():
    results_dir = "results"
    csv_file = "some_results.csv"
    plot_file = "some_plot.png"

    winner_column = "Winner"

    csv_path = os.path.join("..", results_dir, csv_file)
    plot_path = os.path.join("..", results_dir, plot_file)

    if not os.path.exists(csv_path):
        print(f"[ERROR]: {csv_path} doesn't exist")
        return

    data = pd.read_csv(csv_path)

    winners = list(data[winner_column])

    p1_wins = list(filter(lambda winner: winner == "PLAYER1", winners))
    p2_wins = list(filter(lambda winner: winner == "PLAYER2", winners))

    plt.figure(figsize=(10, 6))
    plt.bar(["RandomBot1", "RandomBot2"], [len(p1_wins), len(p2_wins)])

    plt.title("RandomBot vs RandomBot")
    plt.ylabel("Win Rate")

    plt.savefig(plot_path)


if __name__ == "__main__":
    main()
