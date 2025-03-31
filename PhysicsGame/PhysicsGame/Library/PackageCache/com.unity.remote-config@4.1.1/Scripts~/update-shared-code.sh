mkdir -p tmp

starbuck2 slink com.unity.services.shared tmp -r MyService -n RemoteConfig.Authoring.Editor.Shared \
  -m Runtime/Serialization \
  Editor/Assets \
  Editor/Infrastructure/Collections \
  Editor/EditorUtils \
  Editor/Crypto \
  Editor/DependencyInversion \
  Editor/Logging

cp -r tmp/Editor/Shared/* ../Editor/Authoring/Shared
cp -r tmp/Runtime/Shared/* ../Editor/Authoring/Shared

rm -rf tmp
