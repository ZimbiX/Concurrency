#!/usr/bin/env ruby

class Array
  def drop_last
    self[0...-1]
  end
end

module InitAttrs
  def init_attrs attrs, options = {}
    options = {
      prefix: ''
    }.merge options
    attrs.each { |attr, val| send "#{options[:prefix]}#{attr}=".to_sym, val }
  end
end

class FileFinder
  attr_accessor :search_ext, :search_location
  include InitAttrs

  def initialize attrs
    init_attrs attrs, prefix: "search_"
  end

  def find
    Dir.glob(File.join search_location, "*.#{search_ext}").sort.map do |filename|
      filename_to_file filename
    end
  end

  def filename_to_file filename
    File.new({
      name: filename_without_ext(filename),
      ext: search_ext,
      location: file_location(filename),
    })
  end

  def filename_without_ext filename
    file_path_pieces(filename).last.gsub(/\.#{search_ext}$/, '')
  end

  def file_location filename
    File.join(*file_path_pieces(filename).drop_last)
  end

  def file_path_pieces filename
    filename.split(/\/|\\/)
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
  arg_location = ARGV[0] || '.'
  arg_ext      = ARGV[1] || '*'
  FileFinder.new(ext: arg_ext, location: arg_location).find.map do |file|
    print "- [", file.name, "](", file.path, ")"; puts
  end
end