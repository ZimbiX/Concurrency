#!/usr/bin/env ruby

# Create logs using something like:
# HilzerBarbershop/bin/Debug/HilzerBarbershop.exe 2>&1 | tee hilzer.log

require 'yaml'

def replace_fg_colors! contents, before, after
  contents.gsub!("<span style=\"color:#{before};\">", "<span style=\"color:#{after};\">")
end

def insert_css! contents, css_insertion
  css_insertion = '<style type="text/css">' + css_insertion + '</style>'
  contents.gsub! /( *<\/head>\n *<body>)/, "#{css_insertion}\\1"
end

def insert_css_font_size! contents, font_size
  insert_css! contents, "* { font-size: #{font_size} !important; }"
end

if __FILE__ == $0

  log_file_names = ARGV.empty? ? `ls *.log`.split : ARGV
  # log_file_names = ["LightSwitch.log"]

  html_file_names = log_file_names.map do |log_file_name|
    html_file_name = log_file_name.gsub('.log', '.htm')
    `ansifilter -i #{log_file_name} -o #{html_file_name} -H`
    html_file_name
  end

  html_file_names.each do |html_file_name|
    File.open(html_file_name) do |file|
      contents = file.read
      log_file_name = html_file_name.gsub('.htm', '.log')
      log_contents = File.read log_file_name
      # Remove ANSI colours
      log_contents.gsub!(/\e\[[0-9]+m/, '')

      # puts
      puts html_file_name
      # puts log_file_name
      widths = log_contents.scan(/^[\-=]{5,}$/).map { |x| x.length }
      widths += log_contents.scan(/^.{4} {10,}.+$/).map { |x| x.length }
      # puts widths
      max_file_width = widths.max || 0
      # puts max_file_width

      # Fix font size for width -- 10pt by default
      width_font_size_maps = {
        82 => '10pt',
        93 => '9pt',
        109 => '7.5pt',
        131 => '7pt',
        164 => '7px',
        219 => '5px'
      }
      puts ' ' * 20 + "Max file width of #{max_file_width}"
      width_font_size_maps.keys.sort.each do |max_width|
        if max_file_width <= max_width
          font_size = width_font_size_maps[max_width]
          insert_css_font_size! contents, font_size
          puts ' ' * 20 + "- less than #{max_width}, so using font size: #{font_size}"
          break
        end
      end

      insert_css! contents, 'pre { overflow-wrap: break-word; }'
      
      replace_fg_colors! contents, '#f0f0f0', '#888' # White to grey
      replace_fg_colors! contents, '#00f000', '#008100' # Light green to dark green
      replace_fg_colors! contents, '#f0f000', '#F17E00' # Light yellow to orange
      replace_fg_colors! contents, '#00f0f0', '#02A1A2' # Light blue to mid blue

      File.open(file, "w+") { |f| f.write(contents) }
    end
  end

  html_file_names.each do |html_file_name|
    `google-chrome #{html_file_name}`
  end

end