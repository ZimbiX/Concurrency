#!/usr/bin/env ruby

module InitAttrs
  def init_attrs attrs
    attrs.each { |a, val| send "#{a}=".to_sym, val }
  end
end

class FileFinder
  attr_accessor :ext, :location
  include InitAttrs

  def initialize attrs
    init_attrs attrs
  end

  def find
    Dir.glob(File.join location, "*.#{ext}").sort.map do |filename|
      filename_to_file filename
    end
  end

  def filename_to_file filename
    File.new({
      name: filename_without_ext(filename),
      ext: ext,
      location: location,
    })
  end

  def filename_without_ext filename
    filename.split(/\/|\\/).last.gsub(/\.#{ext}$/, '')
  end
end

class File
  attr_accessor :name, :ext, :location
  include InitAttrs

  def initialize attrs
    init_attrs attrs
  end

  def path
    File.join location, filename
  end

  def filename
    "#{name}.#{ext}"
  end
end

if __FILE__ == $0
  location = File.join(File.dirname(__FILE__), "logs_new")
  FileFinder.new(ext: "htm", location: location).find.map do |file|
    print "- [", file.name, "](", file.path, ")"; puts
  end
end