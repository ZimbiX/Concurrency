#!/usr/bin/env ruby

solution_name = "ConcurrencyUtilities"

project_dirs = %w[
  CigaretteSmokers
  ConcurrencyUtilities
  DiningPhilosophers
  HilzerBarbershop
  RiverCrossing
  TestConcurrencyUtilities
  ZorkServer
]

def sep
  puts '', '-' * 80, ''
end



if __FILE__ == $0

  if ARGV[0] != '-s' # Check if we need to skip building
    puts "Removing all existing builds..."
    system "rm -rf */bin/Debug"

    puts "Building projects from source..."
    project_dirs.each do |project_dir|
      sep
      puts "Building projects for: #{project_dir}...", ''
      Dir.chdir project_dir do
        system "xbuild"
        if $?.exitstatus != 0
          sep
          puts "Build error detected; exiting"
          exit 1
        end
      end
    end
  end

  sep
  puts "Completed building projects from source"

  time = Time.now.strftime "%Y-%m-%d_%H-%M-%S"
  zip_file = "#{solution_name}_build_#{time}.zip"
  puts "Compressing builds to: #{zip_file}..."
  dirs = project_dirs.map { |dir| "#{dir}/bin/Debug" }.join(' ')
  system "zip -r -0 -v #{zip_file} #{dirs}"

  sep
  puts "Finished building and packaging release: #{zip_file}"
end