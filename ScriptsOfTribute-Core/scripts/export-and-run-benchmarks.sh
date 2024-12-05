#!/bin/bash

server="$1"
remote_path="/home/arch/"
tar_file="sot.tar.gz"
docker_file="sot.tar"

# Uncomment the following lines if Docker build and save are required
# sudo docker build -t sot . || { echo "Docker build failed"; exit 1; }
# sudo docker save -o "$docker_file" sot || { echo "Docker save failed"; exit 1; }

dir_to_copy=("../Benchmarks" "../Bots" "../Engine" "../GameRunner" "../HPO")

for file in "${dir_to_copy[@]}"; do
    if [ ! -e "$file" ]; then
        echo "Error: $file does not exist"
        exit 1
    fi
done

tar --exclude="bin" --exclude="obj" -czvf "$tar_file" "${dir_to_copy[@]}" || { echo "Failed to create tar archive"; exit 1; }

scp -r "$tar_file" "$server":"$remote_path" || { echo "SCP failed"; exit 1; }
ssh -t "$server" "cd $remote_path; tar -xzf $tar_file; rm $tar_file" || { echo "Archive extraction failed"; exit 1; }
# ssh -t "$server" "cd $remote_path; tar -xzf $tar_file; rm $tar_file; cp Engine/cards.json .; srun singularity exec sot.sif bash -c 'for benchmark in ${benchmarks[@]}; do dotnet run --project \$benchmark; done'" || { echo "Remote execution failed"; exit 1; }

rm "$tar_file"
